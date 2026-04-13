using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantManagement
{
    public partial class Auth : Form
    {
        public Auth()
        {
            InitializeComponent();
        }

        private void Auth_Load(object sender, EventArgs e)
        {
            this.ActiveControl = cnie_auth;
            LoadRememberMe();
        }

        private void btnloginAuth_Click(object sender, EventArgs e)
        {
            string cnie = cnie_auth.Text.Trim();
            string password = password_auth.Text.Trim();

            if (string.IsNullOrWhiteSpace(cnie) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter CNIE and Password");
                return;
            }

            if (cnie.Length != 8)
            {
                MessageBox.Show("CNIE must be 8 characters long");
                return;
            }

            string hashedPassword = PasswordHelper.ComputeHash(password);

            using (EFDBEntities db = new EFDBEntities())
            {
                var user = db.Users.FirstOrDefault(x =>
                    x.CNIE == cnie &&
                    x.PasswordHash == hashedPassword);

                if (user == null)
                {
                    MessageBox.Show("Invalid CNIE or Password");
                    return;
                }

                if (user.IsActive == null || user.IsActive.Trim().ToLower() != "isactive")
                {
                    MessageBox.Show("This account is inactive");
                    return;
                }

                CurrentUser.CNIE = user.CNIE;
                CurrentUser.Username = user.LastName + " " + user.FIrstName;
                CurrentUser.Role = user.Role;
                CurrentUser.Image = user.Image;

                SaveRememberMe();
                ActivityLogger.Log("Login", "Auth", null);

                Dashboard dash = new Dashboard();
                dash.Show();
                this.Hide();
            }
        }

        private void btn_exit_auth_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void chk_showpassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_showpassword.Checked)
            {
                password_auth.PasswordChar = '\0';
            }
            else
            {
                password_auth.PasswordChar = '*';
            }
        }

        private void SaveRememberMe()
        {
            if (chk_rememberme.Checked)
            {
                Properties.Settings.Default.RememberMe = true;
                Properties.Settings.Default.CNIE = cnie_auth.Text.Trim();
            }
            else
            {
                Properties.Settings.Default.RememberMe = false;
                Properties.Settings.Default.CNIE = "";
            }

            Properties.Settings.Default.Save();
        }

        private void LoadRememberMe()
        {
            if (Properties.Settings.Default.RememberMe)
            {
                chk_rememberme.Checked = true;
                cnie_auth.Text = Properties.Settings.Default.CNIE;
                password_auth.Text = "";
            }
            else
            {
                chk_rememberme.Checked = false;
                cnie_auth.Text = "";
                password_auth.Text = "";
            }
        }
    }
}