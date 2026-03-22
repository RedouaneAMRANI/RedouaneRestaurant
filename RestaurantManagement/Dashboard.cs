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

namespace RestaurantManagement
{
    public partial class Dashboard : Form
    {

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            labelDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            //Timer date and time
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            //Tabcontrol
            dash.Visible = true;
            orders.Visible = false;
            menu.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            receiptprint.Visible = false;
        }

        private void btn_dashboard_Click(object sender, EventArgs e)
        {
            dash.Visible = true;
            orders.Visible = false;
            menu.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            receiptprint.Visible = false;
        }

        private void btn_orders_Click(object sender, EventArgs e)
        {
            dash.Visible = false;
            orders.Visible = true;
            menu.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            receiptprint.Visible = false;
        }

        private void btn_menu_Click(object sender, EventArgs e)
        {
            dash.Visible = false;
            orders.Visible = false;
            menu.Visible = true;
            staff.Visible = false;
            payment.Visible = false;
            receiptprint.Visible = false;
        }

        private void btn_staff_Click(object sender, EventArgs e)
        {
            dash.Visible = false;
            orders.Visible = false;
            menu.Visible = false;
            staff.Visible = true;
            payment.Visible = false;
            receiptprint.Visible = false;
        }

        private void btn_payment_Click(object sender, EventArgs e)
        {
            dash.Visible = false;
            orders.Visible = false;
            menu.Visible = false;
            staff.Visible = false;
            payment.Visible = true;
            receiptprint.Visible = false;
        }

        private void btn_receiptprint_Click(object sender, EventArgs e)
        {
            dash.Visible = false;
            orders.Visible = false;
            menu.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            receiptprint.Visible = true;
        }

        private void btn_logout_Click(object sender, EventArgs e)
        {
            Form1 auth = new Form1();
            auth.Show();

            this.Hide();
        }
    }
}
