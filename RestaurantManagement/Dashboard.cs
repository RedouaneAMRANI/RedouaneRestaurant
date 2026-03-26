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

        private void btn_logout_Click_1(object sender, EventArgs e)
        {
            Auth auth = new Auth();
            auth.Show();

            this.Hide();
        }
        
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

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void btn_add_order_Click(object sender, EventArgs e)
        {
           PaymentForm paymentForm = new PaymentForm();
            paymentForm.Show();
        }
    }
}