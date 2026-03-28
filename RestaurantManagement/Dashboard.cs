using DevComponents.DotNetBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace RestaurantManagement
{
    public partial class Dashboard : Form
    {
        User model = new User();

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            picture_user.Image = picture_user.BackgroundImage;
            picture_user.BackgroundImageLayout = ImageLayout.Stretch;

            LoadData();

            //Timer date and time
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            //Tabcontrol
            ActivateButton(btn_dashboard);
            dash.Visible = true;
            orders.Visible = false;
            ordersbysite.Visible = false;
            menu.Visible = false;
            products.Visible = false;   
            staff.Visible = false;
            payment.Visible = false;
            history.Visible = false;   
        }

        //////////////////// Timer date and time
        private void Timer1_Tick(object sender, EventArgs e)
        {
            labelDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        //////////////////// Logout button
        private void btn_logout_Click(object sender, EventArgs e)
        {
            Auth auth = new Auth();
            auth.Show();
            this.Close();
        }

        //////////////////// Change button color when clicked
        Guna.UI2.WinForms.Guna2Button currentButton;
        private void ActivateButton(object sender)
        {
            if (currentButton != null)
            {
                currentButton.FillColor = Color.FromArgb(26, 43, 74); // Blue
            }
            currentButton = (Guna.UI2.WinForms.Guna2Button)sender;
            currentButton.FillColor = Color.FromArgb(199, 161, 122); // Gold
        }

        //////////////////// Tabcontrol
        private void btn_dashboard_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            dash.Visible = true;
            orders.Visible = false;
            menu.Visible = false;
            products.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            history.Visible = false;
        }
        private void btn_orders_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            orders.Visible = true;
            dash.Visible = false;
            menu.Visible = false;
            products.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            history.Visible = false;
        }
        private void btn_menu_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            btn_categories_menu.FillColor = Color.FromArgb(199, 161, 122); // Gold
            menu.Visible = true;
            products.Visible = false;
            dash.Visible = false;
            orders.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            history.Visible = false;
        }
        private void btn_staff_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            staff.Visible = true;
            menu.Visible = false;
            products.Visible = false;
            orders.Visible = false;
            dash.Visible = false;
            payment.Visible = false;
            history.Visible = false;
        }
        private void btn_payment_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            payment.Visible = true;
            staff.Visible = false;
            menu.Visible = false;
            products.Visible = false;
            orders.Visible = false;
            dash.Visible = false;
            history.Visible = false;
        }
        private void btn_history_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            history.Visible = true;
            payment.Visible = false;
            staff.Visible = false;
            menu.Visible = false;
            products.Visible = false;
            orders.Visible = false;
            dash.Visible = false;
        }

        //////////////////// Switch between menu and products
        private void btn_products_menu_Click(object sender, EventArgs e)
        {
            btn_products_products.FillColor = Color.FromArgb(199, 161, 122); // Gold

            products.Visible = true;
            menu.Visible = false;
        }
        private void btn_categories_products_Click(object sender, EventArgs e)
        {
            btn_categories_menu.FillColor = Color.FromArgb(199, 161, 122); // Gold

            products.Visible = false;
            menu.Visible = true;
        }

        //////////////////// Browse image for user
        private void browse_user_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.FileName = "";
            this.openFileDialog1.ShowDialog();
            if (this.openFileDialog1.FileName != "")
                this.picture_user.ImageLocation = this.openFileDialog1.FileName;
        }
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        //////////////////// USER FORM
        void Clear()
        {
            cnie.Text = lastname.Text = firstname.Text = password.Text = "";
            role.SelectedIndex = -1;
            status_user.SelectedIndex = -1;
            picture_user.Image = picture_user.BackgroundImage;
            picture_user.BackgroundImageLayout = ImageLayout.Stretch;
            save_user.Text = "Save";
        }
        private void clear_user_Click(object sender, EventArgs e)
        {
            Clear();
        }

        //////////////////// Load data to datagridview
        void LoadData()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                dgvUser.DataSource = db.Users.ToList<User>();
            }
        }

        private void save_user_Click(object sender, EventArgs e)
        {
            if (cnie.Text != "" && lastname.Text != "" && firstname.Text != "" && password.Text != "" && role.Text != "" && status_user.Text != "")
            {
                model.CNIE = cnie.Text;
                model.LastName = lastname.Text;
                model.FIrstName = firstname.Text.Trim();
                model.PasswordHash = password.Text.Trim();
                model.Role = role.Text;
                model.IsActive = status_user.Text;
                model.CreatedAt = DateTime.Now;
                if (!string.IsNullOrEmpty(picture_user.ImageLocation))
                {
                    model.Image = File.ReadAllBytes(picture_user.ImageLocation);
                }
                else
                {
                    MessageBox.Show("Please fill in all fields");
                }

                using (EFDBEntities db = new EFDBEntities())
                {
                    var existingUser = db.Users.Find(model.CNIE);
                    if (existingUser == null)
                    {
                        db.Users.Add(model);
                    }
                    else
                    {
                        if (existingUser != null)
                        {
                            existingUser.LastName = model.LastName;
                            existingUser.FIrstName = model.FIrstName;
                            existingUser.PasswordHash = model.PasswordHash;
                            existingUser.Role = model.Role;
                            existingUser.IsActive = model.IsActive;
                            if (!string.IsNullOrEmpty(picture_user.ImageLocation))
                            {
                                existingUser.Image = File.ReadAllBytes(picture_user.ImageLocation);
                            }
                        }
                    }
                    db.SaveChanges();
                }
                Clear();
                LoadData();
                MessageBox.Show("Submitted successfully!");
            }
            else
            {
                MessageBox.Show("Please fill in all fields");
            }
        }

        private void search_user_Click(object sender, EventArgs e)
        {
            if (cnie.Text.Trim() != "")
            {
                using (var db = new EFDBEntities())
                {
                    model.CNIE = cnie.Text;
                    var user = db.Users.Find(model.CNIE);
                    if (user != null)
                    {
                        cnie.Text = user.CNIE;
                        lastname.Text = user.LastName;
                        firstname.Text = user.FIrstName;
                        password.Text = user.PasswordHash;
                        role.Text = user.Role;
                        status_user.Text = user.IsActive;

                        if (user.Image != null)
                        {
                            using (MemoryStream ms = new MemoryStream(user.Image))
                            {
                                picture_user.Image = Image.FromStream(ms);
                            }
                        }
                        save_user.Text = "Update";
                    }
                    else
                    {
                        MessageBox.Show("No user found");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a CNIE to search");
            }
        }

        private void btn_add_order_Click(object sender, EventArgs e)
        {
            PaymentForm paymentForm = new PaymentForm();
            paymentForm.Show();
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}