using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantManagement
{
    public partial class CartProducts : Form
    {
        public event EventHandler<int> OnDeleteProduct;
        public event EventHandler<(int ProductId, int Quantity)> OnQuantityChangedProduct;
        
        public CartProducts()
        {
            InitializeComponent();
        }

        public void LoadCartProducts(List<CartItemModel> cartItems)
        {
            flowLayoutPanel_cart.Controls.Clear();

            foreach (var item in cartItems)
            {
                UC_CartProduct uc = new UC_CartProduct();

                uc.ProductId = item.ProductId;
                uc.lbl_product_name.Text = item.ProductName;
                uc.lbl_price.Text = item.UnitPrice.ToString("0.00");
                uc.num_quantity.Value = item.Quantity;
                uc.lbl_total.Text = item.Total.ToString("0.00");

                if (item.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream(item.Image))
                    {
                        uc.pic_product.Image = Image.FromStream(ms);
                    }
                }

                uc.OnDelete += Uc_OnDelete;
                uc.OnQuantityChanged += Uc_OnQuantityChanged;

                flowLayoutPanel_cart.Controls.Add(uc);
            }
        }

        public void LoadCartProductsReadOnly(List<CartItemModel> items)
        {
            flowLayoutPanel_cart.Controls.Clear();

            foreach (var item in items)
            {
                UC_CartProduct uc = new UC_CartProduct();

                uc.ProductId = item.ProductId;
                uc.lbl_product_name.Text = item.ProductName;
                uc.lbl_price.Text = item.UnitPrice.ToString("0.00");
                uc.num_quantity.Value = item.Quantity;
                uc.lbl_total.Text = item.Total.ToString("0.00");

                if (item.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream(item.Image))
                    {
                        uc.pic_product.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    uc.pic_product.Image = uc.pic_product.BackgroundImage;
                }

                // read only
                uc.num_quantity.Enabled = false;
                uc.btn_delete.Visible = false;

                flowLayoutPanel_cart.Controls.Add(uc);
            }
        }

        private void Uc_OnDelete(object sender, int productId)
        {
            if (OnDeleteProduct != null)
            {
                OnDeleteProduct(this, productId);
            }
        }

        private void Uc_OnQuantityChanged(object sender, (int ProductId, int Quantity) data)
        {
            if (OnQuantityChangedProduct != null)
            {
                OnQuantityChangedProduct(this, data);
            }
        }
    }
}
