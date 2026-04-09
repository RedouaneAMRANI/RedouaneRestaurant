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
    public partial class UC_CartProduct : UserControl
    {
        public int ProductId { get; set; }

        public event EventHandler<int> OnDelete;
        public event EventHandler<(int ProductId, int Quantity)> OnQuantityChanged;

        public UC_CartProduct()
        {
            InitializeComponent();

            btn_delete.LinkClicked += btn_delete_LinkClicked;
            num_quantity.ValueChanged += num_quantity_ValueChanged;
        }

        private void UC_CartProduct_Load(object sender, EventArgs e)
        {

        }

        private void btn_delete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (OnDelete != null)
            {
                OnDelete(this, ProductId);
            }
        }

        private void num_quantity_ValueChanged(object sender, EventArgs e)
        {
            float price = 0;
            int qty = Convert.ToInt32(num_quantity.Value);

            float.TryParse(lbl_price.Text, out price);

            lbl_total.Text = (price * qty).ToString("0.00");

            if (OnQuantityChanged != null)
            {
                OnQuantityChanged(this, (ProductId, qty));
            }
        }
    }
}
