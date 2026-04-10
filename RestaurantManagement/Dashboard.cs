using DevComponents.DotNetBar;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;
using Padding = System.Windows.Forms.Padding;

namespace RestaurantManagement
{
    public partial class Dashboard : Form
    {
        User model_User = new User();
        Product model_Products = new Product();
        RestaurantTable model_Table = new RestaurantTable();
        Reservation model_Reservation = new Reservation();
        Order model_Order = new Order();
        OrderItem model_OrderItem = new OrderItem();
        private List<CartItemModel> cartItems = new List<CartItemModel>();
        private int selectedProductId = 0;
        private float selectedProductPrice = 0;
        private byte[] selectedProductImage = null;
        private string selectedProductName = "";
        private CartProducts cartForm;
        private bool isLoadingOrderData = false;
        private int selectedPaymentOrderId = 0;

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            picture_user.Image = picture_user.BackgroundImage;
            picture_user.BackgroundImageLayout = ImageLayout.Stretch;

            picture_products.Image = picture_products.BackgroundImage;
            picture_products.BackgroundImageLayout = ImageLayout.Stretch;

            LoadUser();
            LoadProducts();
            LoadTables();
            LoadOrders_Products();
            LoadPaymentSearchBy();
            LoadPaymentHistory();

            dgv_payment_history.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_payment_history.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_payment_history.MultiSelect = false;
            dgv_payment_history.ReadOnly = true;
            dgv_payment_history.AllowUserToAddRows = false;

            //Timer date and time
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
            // Set current date and time to datepicker
            datereservation.MinDate = DateTime.Now;

            //Tabcontrol
            ActivateButton(btn_dashboard);
            dash.Visible = true;
            orders.Visible = false;
            ordersbysite.Visible = false;
            products.Visible = false;
            table.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            history.Visible = false;

            // Reservations tables double click event
            table1.DoubleClick += Panel_DoubleClick;
            table2.DoubleClick += Panel_DoubleClick;
            table3.DoubleClick += Panel_DoubleClick;
            table4.DoubleClick += Panel_DoubleClick;
            table5.DoubleClick += Panel_DoubleClick;
            table6.DoubleClick += Panel_DoubleClick;
            table7.DoubleClick += Panel_DoubleClick;
            table8.DoubleClick += Panel_DoubleClick;
            table9.DoubleClick += Panel_DoubleClick;
            table10.DoubleClick += Panel_DoubleClick;
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
                currentButton.FillColor = Color.White;
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
            products.Visible = false;
            table.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            history.Visible = false;
        }

        private void btn_orders_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            orders.Visible = true;
            dash.Visible = false;
            products.Visible = false;
            table.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            history.Visible = false;
        }

        private void btn_menu_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            products.Visible = true;
            table.Visible = false;
            orders.Visible = false;
            dash.Visible = false;
            staff.Visible = false;
            payment.Visible = false;
            history.Visible = false;
        }

        private void btn_staff_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            staff.Visible = true;
            products.Visible = false;
            table.Visible = false;
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
            table.Visible = false;
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
            table.Visible = false;
            products.Visible = false;
            orders.Visible = false;
            dash.Visible = false;
        }

        private void btn_tables_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            table.Visible = true;
            history.Visible = false;
            payment.Visible = false;
            staff.Visible = false;
            products.Visible = false;
            orders.Visible = false;
            dash.Visible = false;
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
            password.Enabled = true;
            role.SelectedIndex = -1;
            status_user.SelectedIndex = -1;
            picture_user.Image = picture_user.BackgroundImage;
            picture_user.BackgroundImageLayout = ImageLayout.Stretch;
            btn_save_user.Text = "Save";
            LoadUser();
        }

        private void clear_user_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void LoadUser()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                dgvUser.DataSource = db.Users
                    .Select(u => new
                    {
                        u.CNIE,
                        u.LastName,
                        u.FIrstName,
                        u.Role,
                        u.Image,
                        u.IsActive,
                        u.CreatedAt
                    })
                    .ToList();
            }
        }

        public static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashbyte = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashbyte).Replace("-", "").ToLower();
            }
        }

        private void lastname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void firstname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void save_user_Click(object sender, EventArgs e)
        {
            if (cnie.Text != "" && lastname.Text != "" && firstname.Text != "" && role.Text != "" && status_user.Text != "")
            {
                if (cnie.Text.Length == 8)
                {
                    model_User.CNIE = cnie.Text.Trim();
                } 
                else
                { 
                    MessageBox.Show("CNIE must be 8 characters long");
                    return; 
                }
                if (Regex.IsMatch(lastname.Text, @"^[a-zA-Z0-9]+$"))
                {
                    model_User.LastName = lastname.Text.Trim();
                }
                else
                {
                    MessageBox.Show("LastName must contain only letters");
                    return;
                }
                if (Regex.IsMatch(firstname.Text, @"^[a-zA-Z0-9]+$"))
                {
                    model_User.FIrstName = firstname.Text.Trim();
                }
                else
                {
                    MessageBox.Show("FirstName must contain only letters");
                    return;
                }
                model_User.Role = role.Text;
                model_User.IsActive = status_user.Text;
                model_User.CreatedAt = DateTime.Now;
                if (!string.IsNullOrEmpty(picture_user.ImageLocation))
                {
                    model_User.Image = File.ReadAllBytes(picture_user.ImageLocation);
                }
                using (EFDBEntities db = new EFDBEntities())
                {
                    var existingUser = db.Users.Find(model_User.CNIE);
                    if (existingUser == null)
                    {
                        if (!password.Text.All(char.IsDigit))
                        {
                            MessageBox.Show("Password must contain only numbers");
                            return;
                        }
                        if (password.Text.Length == 4)
                        {
                            model_User.PasswordHash = ComputeHash(password.Text.Trim());
                            db.Users.Add(model_User);
                        }
                        else
                        {
                            MessageBox.Show("Password must be 4 characters long");
                            return;
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Are you Sure to Update this Record", "Message", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                        existingUser.LastName = model_User.LastName;
                        existingUser.FIrstName = model_User.FIrstName;
                        existingUser.Role = model_User.Role;
                        existingUser.IsActive = model_User.IsActive;

                        if (!string.IsNullOrEmpty(password.Text))
                        {
                            existingUser.PasswordHash = ComputeHash(password.Text.Trim());
                        }

                        if (!string.IsNullOrEmpty(picture_user.ImageLocation))
                        {
                            existingUser.Image = File.ReadAllBytes(picture_user.ImageLocation);
                        }
                    }
                    db.SaveChanges();
                }
                Clear();
                MessageBox.Show("Submitted successfully!");
            }
            else
            {
                MessageBox.Show("Please fill in all fields");
            }
        }

        private void search_user_Click(object sender, EventArgs e)
        {
            string cnieText = cnie.Text.Trim();
            string firstNameText = firstname.Text.Trim();

            using (var db = new EFDBEntities())
            {
                if (!string.IsNullOrEmpty(cnieText))
                {
                    var user = db.Users.Find(cnieText);

                    if (user != null)
                    {
                        cnie.Text = user.CNIE;
                        lastname.Text = user.LastName;
                        firstname.Text = user.FIrstName;
                        password.Enabled = false;
                        role.Text = user.Role;
                        status_user.Text = user.IsActive;

                        if (user.Image != null)
                        {
                            using (MemoryStream ms = new MemoryStream(user.Image))
                            {
                                picture_user.Image = Image.FromStream(ms);
                            }
                        }

                        btn_save_user.Text = "Update";
                    }
                    else
                    {
                        MessageBox.Show("No user found");
                    }

                    return;
                }

                if (!string.IsNullOrEmpty(firstNameText))
                {
                    var filteredUsers = db.Users
                        .Where(u => u.FIrstName.Contains(firstNameText))
                        .Select(u => new
                        {
                            u.CNIE,
                            u.LastName,
                            u.FIrstName,
                            u.Role,
                            u.IsActive,
                            u.CreatedAt
                        })
                        .ToList();

                    if (filteredUsers.Count > 0)
                    {
                        dgvUser.DataSource = null;
                        dgvUser.DataSource = filteredUsers;
                    }
                    else
                    {
                        MessageBox.Show("No user found");
                    }

                    return;
                }
                MessageBox.Show("Enter CNIE or First Name to search");
            }
        }

        private void dgvUser_DoubleClick(object sender, EventArgs e)
        {
            if (dgvUser.CurrentRow.Index != -1)
            {
                cnie.Text = dgvUser.CurrentRow.Cells["CNIE"].Value.ToString();
                lastname.Text = dgvUser.CurrentRow.Cells["LastName"].Value.ToString();
                firstname.Text = dgvUser.CurrentRow.Cells["FIrstName"].Value.ToString();
                role.Text = dgvUser.CurrentRow.Cells["Role"].Value.ToString();
                status_user.Text = dgvUser.CurrentRow.Cells["IsActive"].Value.ToString();
                using (var db = new EFDBEntities())
                {
                    var user = db.Users.Find(cnie.Text);
                    if (user != null && user.Image != null)
                    {
                        using (MemoryStream ms = new MemoryStream(user.Image))
                        {
                            picture_user.Image = Image.FromStream(ms);
                        }
                    }
                }
                password.Enabled = false;
                btn_save_user.Text = "Update";
            }
        }

        //////////////////// Products form
        void ClearProducts()
        {
            name_products.Text = price_products.Text = "";
            filterbycategory.SelectedIndex = -1;
            categories_products.SelectedIndex = -1;
            status_products.SelectedIndex = -1;
            model_Products = new Product();
            picture_products.Image = picture_products.BackgroundImage;
            picture_products.BackgroundImageLayout = ImageLayout.Stretch;
            btn_save_products.Text = "Save";
            LoadProducts();
        }

        private void btn_clear_products_Click(object sender, EventArgs e)
        {
            ClearProducts();
        }

        void LoadProducts()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                dgvProducts.DataSource = db.Products
                    .Select(p => new
                    {
                        p.ProductId,
                        p.Name,
                        p.Price,
                        p.Image,
                        p.Category,
                        p.IsAvailable,
                    })
                    .ToList();
            }
        }

        private void name_products_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btn_save_products_Click(object sender, EventArgs e)
        {
            if (name_products.Text != "" && price_products.Text != "" && categories_products.Text != "" && status_products.Text != "")
            {
                if (Regex.IsMatch(name_products.Text, @"^[a-zA-Z0-9]+$"))
                {
                    model_Products.Name = name_products.Text.Trim();
                }
                else
                {
                    MessageBox.Show("name_products must contain only letters");
                    return;
                }
                if (decimal.TryParse(price_products.Text.Trim(), out decimal price))
                {
                    model_Products.Price = (double)price;
                }
                else
                {
                    MessageBox.Show("Price must be a valid number");
                    return;
                }
                model_Products.Category = categories_products.Text;
                model_Products.IsAvailable = status_products.Text;
                if (!string.IsNullOrEmpty(picture_products.ImageLocation))
                {
                    model_Products.Image = File.ReadAllBytes(picture_products.ImageLocation);
                }
                using (EFDBEntities db = new EFDBEntities())
                {
                    if (model_Products.ProductId == 0)
                    {
                        db.Products.Add(model_Products);
                    }
                    else
                    {
                        var existingProduct = db.Products.Find(model_Products.ProductId);

                        if (existingProduct == null) return;

                        if (MessageBox.Show("Are you Sure to Update this Record", "Message", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;

                        existingProduct.Name = model_Products.Name;
                        existingProduct.Price = model_Products.Price;
                        existingProduct.Category = model_Products.Category;
                        existingProduct.IsAvailable = model_Products.IsAvailable;

                        if (!string.IsNullOrEmpty(picture_products.ImageLocation))
                        {
                            existingProduct.Image = File.ReadAllBytes(picture_products.ImageLocation);
                        }
                    }
                    db.SaveChanges();
                }
                ClearProducts();
                MessageBox.Show("Submitted successfully!");
            }
            else
            {
                MessageBox.Show("Please fill in all fields");
            }
        }

        private void filterbycategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCategory = filterbycategory.Text;

            using (var db = new EFDBEntities())
            {
                var result = db.Products
                               .Where(x => x.Category == selectedCategory)
                               .ToList();

                dgvProducts.DataSource = result;
            }
        }

        private void dgvProducts_DoubleClick(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow.Index != -1)
            {
                int productId = Convert.ToInt32(dgvProducts.CurrentRow.Cells["ProductId"].Value);

                using (var db = new EFDBEntities())
                {
                    var product = db.Products.Find(productId);

                    if (product != null)
                    {
                        model_Products = product;

                        name_products.Text = product.Name;
                        price_products.Text = product.Price.ToString();
                        categories_products.Text = product.Category;
                        status_products.Text = product.IsAvailable;

                        if (product.Image != null)
                        {
                            using (MemoryStream ms = new MemoryStream(product.Image))
                            {
                                picture_products.Image = Image.FromStream(ms);
                            }
                        }

                        btn_save_products.Text = "Update";
                    }
                }
            }
        }

        private void browse_product_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.FileName = "";
            this.openFileDialog1.ShowDialog();
            if (this.openFileDialog1.FileName != "")
                this.picture_products.ImageLocation = this.openFileDialog1.FileName;
        }

        //////////////////// Reservations Tables
        void ClearTables()
        {
            customername.Text = customerphone.Text = "";
            tablenumber.SelectedIndex = -1;
            statusreservation.SelectedIndex = -1;
            datereservation.Value = DateTime.Now;
            model_Table = new RestaurantTable();
            model_Reservation = new Reservation();
            btn_save_restable.Text = "Save";
        }

        private void btn_clear_restable_Click(object sender, EventArgs e)
        {
            ClearTables();
        }

        void LoadTables()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                var tables = db.RestaurantTables.ToList();

                foreach (var table in tables)
                {
                    Guna.UI2.WinForms.Guna2Panel pnl = this.Controls.Find("Table" + table.TableNumber, true)
                                             .FirstOrDefault() as Guna2Panel;

                    if (pnl != null)
                    {
                        switch (table.Status)
                        {
                            case "Free":
                                pnl.FillColor = Color.Green;
                                break;

                            case "Occupied":
                                pnl.FillColor = Color.Red;
                                break;

                            case "Reserved":
                                pnl.FillColor = Color.Orange;
                                break;
                        }

                        foreach (Control ctrl in pnl.Controls)
                        {
                            if (ctrl is Label lbl)
                            {
                                if (lbl.Tag?.ToString() == "status")
                                {
                                    lbl.Text = table.Status;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void customername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void customerphone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btn_save_restable_Click(object sender, EventArgs e)
        {
            if (tablenumber.Text != "" && statusreservation.Text != "")
            {
                int tableNumber = Convert.ToInt32(tablenumber.Text);

                using (EFDBEntities db = new EFDBEntities())
                {
                    var table = db.RestaurantTables
                                  .FirstOrDefault(t => t.TableNumber == tableNumber);

                    if (model_Reservation.ReservationId == 0)
                    {
                        Reservation newReservation = new Reservation();

                        newReservation.TableId = table.TableId;
                        if (Regex.IsMatch(customername.Text, @"^[a-zA-Z0-9]+$"))
                        {
                            newReservation.CustomerName = customername.Text.Trim();
                        }
                        else
                        {
                            MessageBox.Show("customername must contain only letters");
                            return;
                        }
                        if (Regex.IsMatch(customerphone.Text, @"^\d+$"))
                        {
                            newReservation.CustomerPhone = customerphone.Text.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Phone must contain only numbers");
                            return;
                        }
                        newReservation.Status = statusreservation.Text;
                        newReservation.ReservationTime = DateTime.Now;
                        newReservation.Reserveat = datereservation.Value;

                        db.Reservations.Add(newReservation);

                        table.Status = "Reserved";
                    }
                    else
                    {
                        var existingReservation = db.Reservations
                                                    .FirstOrDefault(r => r.ReservationId == model_Reservation.ReservationId);

                        if (existingReservation == null) return;

                        if (MessageBox.Show("Are you sure to update?", "Message", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;

                        existingReservation.TableId = table.TableId;
                        existingReservation.CustomerName = customername.Text.Trim();
                        existingReservation.CustomerPhone = customerphone.Text.Trim();
                        existingReservation.Status = statusreservation.Text;
                        existingReservation.Reserveat = datereservation.Value;

                        if (statusreservation.Text == "Completed")
                            table.Status = "Occupied";
                        else if (statusreservation.Text == "Cancelled" || statusreservation.Text == "NoShow")
                            table.Status = "Free";
                        else
                            table.Status = "Reserved";

                    }
                    db.SaveChanges();
                    LoadTables();
                    //LoadOrders();
                }
                ClearTables();
                MessageBox.Show("Submitted successfully!");
            }
            else
            {
                MessageBox.Show("Please fill in all fields");
            }
        }

        private void Panel_DoubleClick(object sender, EventArgs e)
        {
            Panel pnl = sender as Panel;

            if (pnl == null) return;

            int tableNumber = int.Parse(pnl.Name.Replace("table", ""));

            using (var db = new EFDBEntities())
            {
                var table = db.RestaurantTables
                              .FirstOrDefault(t => t.TableNumber == tableNumber);

                if (table == null) return;

                var reservation = db.Reservations
                                    .Where(r => r.TableId == table.TableId)
                                    .OrderByDescending(r => r.ReservationId)
                                    .FirstOrDefault();

                if (reservation != null)
                {
                    model_Reservation = reservation;

                    customername.Text = reservation.CustomerName;
                    customerphone.Text = reservation.CustomerPhone;
                    tablenumber.Text = table.TableNumber.ToString();
                    statusreservation.Text = reservation.Status;

                    if (reservation.Reserveat != null)
                        datereservation.Value = reservation.Reserveat.Value;
                }
                else
                {
                    MessageBox.Show("No reservation found for this table");
                }
            }
        }

        private void btn_add_order_Click(object sender, EventArgs e)
        {
            PaymentForm paymentForm = new PaymentForm(cartItems,
                    ordertype_orders.Text,
                    status_orders.Text,
                    customername_orders.Text,
                    customerphone_orders.Text,
                    table_orders.SelectedValue,
                    tablereserved_orders.SelectedValue);
            paymentForm.Show();
        }

        //////////////////// Orders

        void ClearOrders()
        {
            isLoadingOrderData = true;

            customername_orders.Text = "";
            customerphone_orders.Text = "";
            categories_orders.SelectedIndex = -1;
            ordertype_orders.SelectedIndex = -1;
            product_orders.SelectedIndex = -1;
            status_orders.SelectedIndex = -1;
            quantity_orders.Value = 1;

            table_orders.DataSource = null;
            tablereserved_orders.DataSource = null;
            table_orders.Enabled = true;
            tablereserved_orders.Enabled = true;

            selectedProductId = 0;
            selectedProductPrice = 0;
            selectedProductImage = null;
            selectedProductName = "";

            cartItems.Clear();
            RefreshCartForm();
            lbl_cart_count.Text = "0";

            model_Order = new Order();
            model_OrderItem = new OrderItem();
            btn_save_order.Text = "Add Order";

            isLoadingOrderData = false;
        }

        private void btn_clear_order_Click(object sender, EventArgs e)
        {
            ClearOrders();
        }

        void LoadOrders_Products()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                dgv_orders.DataSource = db.Orders
                        .OrderByDescending(o => o.OrderId)
                        .Select(o => new
                        {
                            o.OrderId,
                            o.OrderType,
                            o.Status,
                            o.CustomerName,
                            o.CustomerPhone,
                            o.TableId,
                            o.CreatedAt
                        })
                        .ToList();
            }
        }

        private void categories_orders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (categories_orders.SelectedIndex == -1) return;

            string selectedCategory = categories_orders.Text;

            using (EFDBEntities db = new EFDBEntities())
            {
                var products = db.Products
                                 .Where(p => p.Category == selectedCategory && p.IsAvailable == "Available")
                                 .Select(p => p.Name)
                                 .ToList();

                product_orders.DataSource = null;
                product_orders.DataSource = products;
                product_orders.SelectedIndex = -1;
            }
        }

        private void ordertype_orders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingOrderData)
                return;

            if (ordertype_orders.Text == "TakeAway" || ordertype_orders.Text == "Online")
            {
                table_orders.Enabled = false;
                tablereserved_orders.Enabled = false;
                table_orders.SelectedIndex = -1;
                tablereserved_orders.SelectedIndex = -1;
            }
            else if (ordertype_orders.Text == "DineIn")
            {
                table_orders.Enabled = true;
                tablereserved_orders.Enabled = true;

                LoadOrderTableCombos();
            }
        }

        private void table_orders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (table_orders.Text != "")
            {
                tablereserved_orders.SelectedIndex = -1;
            }
        }

        private void tablereserved_orders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tablereserved_orders.Text != "")
            {
                table_orders.SelectedIndex = -1;
            }
        }

        private void product_orders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (product_orders.SelectedIndex == -1 || string.IsNullOrWhiteSpace(product_orders.Text))
                return;

            string productName = product_orders.Text.Trim();

            using (EFDBEntities db = new EFDBEntities())
            {
                var product = db.Products.FirstOrDefault(p => p.Name == productName && p.IsAvailable == "Available");

                if (product != null)
                {
                    selectedProductId = product.ProductId;
                    selectedProductPrice = (float)product.Price;
                    selectedProductName = product.Name;
                    selectedProductImage = product.Image;
                }
            }
        }

        private void UpdateCartCount()
        {
            lbl_cart_count.Text = cartItems.Count.ToString();
        }

        private void Uc_OnDelete(object sender, int productId)
        {
            var item = cartItems.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                cartItems.Remove(item);
            }

            RefreshCartForm();
            UpdateCartCount();
        }

        private void Uc_OnQuantityChanged(object sender, (int ProductId, int Quantity) data)
        {
            var item = cartItems.FirstOrDefault(x => x.ProductId == data.ProductId);

            if (item != null)
            {
                item.Quantity = data.Quantity;
            }

            RefreshCartForm();
            UpdateCartCount();
        }

        private void add_product_Click(object sender, EventArgs e)
        {
            if (product_orders.Text == "")
            {
                MessageBox.Show("Please select a product");
                return;
            }

            if (selectedProductId == 0)
            {
                MessageBox.Show("Invalid product selected");
                return;
            }

            int qty = Convert.ToInt32(quantity_orders.Value);

            if (qty <= 0)
            {
                MessageBox.Show("Quantity must be greater than 0");
                return;
            }

            var existingItem = cartItems.FirstOrDefault(x => x.ProductId == selectedProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += qty;
            }
            else
            {
                CartItemModel item = new CartItemModel();
                item.ProductId = selectedProductId;
                item.ProductName = selectedProductName;
                item.UnitPrice = selectedProductPrice;
                item.Quantity = qty;
                item.Image = selectedProductImage;

                cartItems.Add(item);
            }

            RefreshCartForm();
            UpdateCartCount();

            product_orders.SelectedIndex = -1;
            product_orders.Text = "";
            quantity_orders.Value = 1;

            selectedProductId = 0;
            selectedProductPrice = 0;
            selectedProductImage = null;
            selectedProductName = "";
        }

        private void btn_cart_product_Click(object sender, EventArgs e)
        {
            if (cartForm == null || cartForm.IsDisposed)
            {
                cartForm = new CartProducts();
                cartForm.OnDeleteProduct += Uc_OnDelete;
                cartForm.OnQuantityChangedProduct += Uc_OnQuantityChanged;
            }

            cartForm.LoadCartProducts(cartItems);
            cartForm.Show();
            cartForm.BringToFront();
        }

        private void RefreshCartForm()
        {
            if (cartForm != null && !cartForm.IsDisposed)
            {
                cartForm.LoadCartProducts(cartItems);
            }
        }

        private void btn_save_order_Click(object sender, EventArgs e)
        {
            if (ordertype_orders.Text != "" && status_orders.Text != "")
            {
                if (cartItems.Count == 0)
                {
                    MessageBox.Show("Please add at least one product");
                    return;
                }

                PaymentForm frm = new PaymentForm(
                   cartItems,
                   ordertype_orders.Text,
                   status_orders.Text,
                   customername_orders.Text,
                   customerphone_orders.Text,
                   table_orders.SelectedValue,
                   tablereserved_orders.SelectedValue
                );

                DialogResult result = frm.ShowDialog();

                if (result == DialogResult.OK && frm.PaymentCompleted)
                {
                    ClearOrders();
                    LoadOrders_Products();
                }
            }
            else
            {
                MessageBox.Show("Please fill in all required fields");
            }
        }

        private void quantite_product_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dgv_orders_DoubleClick(object sender, EventArgs e)
        {
            if (dgv_orders.CurrentRow == null || dgv_orders.CurrentRow.Index == -1)
                return;

            int orderId = Convert.ToInt32(dgv_orders.CurrentRow.Cells["OrderId"].Value);

            using (var db = new EFDBEntities())
            {
                var order = db.Orders.Find(orderId);

                if (order == null)
                {
                    MessageBox.Show("No order found");
                    return;
                }

                isLoadingOrderData = true;

                model_Order = order;

                ordertype_orders.Text = order.OrderType;
                status_orders.Text = order.Status;
                customername_orders.Text = order.CustomerName;
                customerphone_orders.Text = order.CustomerPhone;

                if (order.OrderType == "DineIn")
                {
                    table_orders.Enabled = true;
                    tablereserved_orders.Enabled = true;

                    LoadOrderTableCombos(order.TableId);

                    if (order.TableId != null)
                    {
                        bool isReservedTable = db.Reservations
                                                 .Any(r => r.TableId == order.TableId.Value && r.Status == "Reserved");

                        if (isReservedTable)
                        {
                            tablereserved_orders.SelectedValue = order.TableId.Value;
                            table_orders.SelectedIndex = -1;
                        }
                        else
                        {
                            table_orders.SelectedValue = order.TableId.Value;
                            tablereserved_orders.SelectedIndex = -1;
                        }
                    }
                    else
                    {
                        table_orders.SelectedIndex = -1;
                        tablereserved_orders.SelectedIndex = -1;
                    }
                }
                else
                {
                    table_orders.Enabled = false;
                    tablereserved_orders.Enabled = false;
                    table_orders.SelectedIndex = -1;
                    tablereserved_orders.SelectedIndex = -1;
                }

                cartItems.Clear();

                var orderItems = db.OrderItems
                                   .Where(x => x.OrderId == order.OrderId)
                                   .ToList();

                foreach (var oi in orderItems)
                {
                    var product = db.Products.FirstOrDefault(p => p.ProductId == oi.ProductId);

                    if (product != null)
                    {
                        CartItemModel item = new CartItemModel();
                        item.ProductId = product.ProductId;
                        item.ProductName = product.Name;
                        item.UnitPrice = (float)oi.UnitPrice;
                        item.Quantity = oi.Quantity;
                        item.Image = product.Image;

                        cartItems.Add(item);
                    }
                }

                isLoadingOrderData = false;
            }

            RefreshCartForm();
            UpdateCartCount();
            btn_save_order.Text = "Update";
        }

        void LoadOrderTableCombos(int? selectedTableId = null)
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                var freeTables = db.RestaurantTables
                                   .Where(t => t.Status == "Free")
                                   .ToList()
                                   .Select(t => new
                                   {
                                       t.TableId,
                                       Display = "Table " + t.TableNumber.ToString() + " Capacity " + t.Capacity.ToString()
                                   })
                                   .ToList();

                var reservedTables = db.Reservations
                                       .Where(r => r.Status == "Reserved")
                                       .Join(db.RestaurantTables,
                                             r => r.TableId,
                                             t => t.TableId,
                                             (r, t) => new
                                             {
                                                 t.TableId,
                                                 t.TableNumber,
                                                 t.Capacity
                                             })
                                       .ToList()
                                       .Select(x => new
                                       {
                                           x.TableId,
                                           Display = "Table " + x.TableNumber.ToString() + " Capacity " + x.Capacity.ToString()
                                       })
                                       .ToList();

                if (selectedTableId != null)
                {
                    bool existsInFree = freeTables.Any(x => x.TableId == selectedTableId.Value);
                    bool existsInReserved = reservedTables.Any(x => x.TableId == selectedTableId.Value);

                    if (!existsInFree && !existsInReserved)
                    {
                        var currentTable = db.RestaurantTables
                                             .Where(t => t.TableId == selectedTableId.Value)
                                             .ToList()
                                             .Select(t => new
                                             {
                                                 t.TableId,
                                                 Display = "Table " + t.TableNumber.ToString() + " Capacity " + t.Capacity.ToString()
                                             })
                                             .FirstOrDefault();

                        if (currentTable != null)
                        {
                            freeTables.Add(currentTable);
                        }
                    }
                }

                table_orders.DataSource = null;
                table_orders.DataSource = freeTables;
                table_orders.DisplayMember = "Display";
                table_orders.ValueMember = "TableId";
                table_orders.SelectedIndex = -1;

                tablereserved_orders.DataSource = null;
                tablereserved_orders.DataSource = reservedTables;
                tablereserved_orders.DisplayMember = "Display";
                tablereserved_orders.ValueMember = "TableId";
                tablereserved_orders.SelectedIndex = -1;
            }
        }

        //////////////////// Payment

        void LoadPaymentHistory()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                var result = db.Orders
                    .GroupJoin(
                        db.Payments,
                        o => o.OrderId,
                        p => p.OrderId,
                        (o, payments) => new { o, payments }
                    )
                    .SelectMany(
                        x => x.payments.DefaultIfEmpty(),
                        (x, p) => new
                        {
                            x.o.OrderId,
                            x.o.CustomerName,
                            x.o.CustomerPhone,
                            x.o.OrderType,
                            x.o.Status,
                            x.o.TableId,
                            x.o.Price,
                            PaymentMethod = p != null ? p.Method : "Not Paid",
                            PaidAt = p != null ? p.PaidAt : null
                        }
                    )
                    .OrderByDescending(x => x.OrderId)
                    .ToList();

                dgv_payment_history.DataSource = result;
            }

            dgv_payment_history.ClearSelection();
        }

        void LoadPaymentSearchBy()
        {
            searchby_payment.Items.Clear();
            searchby_payment.Items.Add("Order ID");
            searchby_payment.Items.Add("Customer Name");
            searchby_payment.Items.Add("Customer Phone");
            searchby_payment.Items.Add("Payment Method");
            searchby_payment.Items.Add("Order Type");
            searchby_payment.Items.Add("Status");
            searchby_payment.SelectedIndex = -1;
        }

        void LoadPaymentOrderItems(int orderId)
        {
            flowLayoutPanel_payment_items.Controls.Clear();

            using (EFDBEntities db = new EFDBEntities())
            {
                var items = db.OrderItems
                    .Where(oi => oi.OrderId == orderId)
                    .Join(
                        db.Products,
                        oi => oi.ProductId,
                        p => p.ProductId,
                        (oi, p) => new
                        {
                            p.Name,
                            oi.Quantity,
                            oi.UnitPrice,
                            Total = oi.Quantity * oi.UnitPrice
                        }
                    )
                    .ToList();

                if (items.Count == 0)
                {
                    Label lblEmpty = new Label();
                    lblEmpty.Text = "No order items found";
                    lblEmpty.AutoSize = true;
                    flowLayoutPanel_payment_items.Controls.Add(lblEmpty);
                    return;
                }

                foreach (var item in items)
                {
                    Panel pnl = new Panel();
                    pnl.Width = flowLayoutPanel_payment_items.Width - 25;
                    pnl.Height = 35;
                    pnl.BorderStyle = BorderStyle.FixedSingle;
                    pnl.Margin = new Padding(3);

                    Label lbl = new Label();
                    lbl.AutoSize = false;
                    lbl.Width = pnl.Width - 10;
                    lbl.Height = 25;
                    lbl.Left = 5;
                    lbl.Top = 5;
                    lbl.Text = item.Name + "   x" + item.Quantity +
                               "   " + Convert.ToDouble(item.UnitPrice).ToString("0,00") +
                               " MAD   =   " + Convert.ToDouble(item.Total).ToString("0,00") + " MAD";

                    pnl.Controls.Add(lbl);
                    flowLayoutPanel_payment_items.Controls.Add(pnl);
                }
            }
        }

        private void btn_search_payment_Click(object sender, EventArgs e)
        {
            string searchBy = searchby_payment.Text.Trim();
            string value = search_payment.Text.Trim();

            using (EFDBEntities db = new EFDBEntities())
            {
                var query = db.Orders
                    .GroupJoin(
                        db.Payments,
                        o => o.OrderId,
                        p => p.OrderId,
                        (o, payments) => new { o, payments }
                    )
                    .SelectMany(
                        x => x.payments.DefaultIfEmpty(),
                        (x, p) => new
                        {
                            x.o.OrderId,
                            x.o.CustomerName,
                            x.o.CustomerPhone,
                            x.o.OrderType,
                            x.o.Status,
                            x.o.TableId,
                            x.o.Price,
                            PaymentMethod = p != null ? p.Method : "Not Paid",
                            PaidAt = p != null ? p.PaidAt : null
                        }
                    );

                if (searchBy == "Order ID")
                {
                    if (value == "")
                    {
                        MessageBox.Show("Enter Order ID");
                        return;
                    }

                    int orderId;
                    if (!int.TryParse(value, out orderId))
                    {
                        MessageBox.Show("Order ID must be a number");
                        return;
                    }

                    query = query.Where(x => x.OrderId == orderId);
                }
                else if (searchBy == "Customer Name")
                {
                    if (value == "")
                    {
                        MessageBox.Show("Enter Customer Name");
                        return;
                    }

                    query = query.Where(x => x.CustomerName.Contains(value));
                }
                else if (searchBy == "Customer Phone")
                {
                    if (value == "")
                    {
                        MessageBox.Show("Enter Customer Phone");
                        return;
                    }

                    query = query.Where(x => x.CustomerPhone.Contains(value));
                }
                else if (searchBy == "Payment Method")
                {
                    if (value == "")
                    {
                        MessageBox.Show("Enter Payment Method");
                        return;
                    }

                    query = query.Where(x => x.PaymentMethod.Contains(value));
                }
                else if (searchBy == "Order Type")
                {
                    if (value == "")
                    {
                        MessageBox.Show("Enter Order Type");
                        return;
                    }

                    query = query.Where(x => x.OrderType.Contains(value));
                }
                else if (searchBy == "Status")
                {
                    if (value == "")
                    {
                        MessageBox.Show("Enter Status");
                        return;
                    }

                    query = query.Where(x => x.Status.Contains(value));
                }
                else
                {
                    MessageBox.Show("Please select Search By");
                    return;
                }

                dgv_payment_history.DataSource = query
                    .OrderByDescending(x => x.OrderId)
                    .ToList();
            }

            dgv_payment_history.ClearSelection();
            flowLayoutPanel_payment_items.Controls.Clear();
            selectedPaymentOrderId = 0;
        }

        private void btn_clear_payment_Click(object sender, EventArgs e)
        {
            searchby_payment.SelectedIndex = -1;
            search_payment.Clear();
            selectedPaymentOrderId = 0;

            flowLayoutPanel_payment_items.Controls.Clear();
            LoadPaymentHistory();
        }

        private void btn_print_payment_Click(object sender, EventArgs e)
        {
            if (selectedPaymentOrderId == 0)
            {
                MessageBox.Show("Please double click an order first");
                return;
            }

            try
            {
                string folder = Path.Combine(Application.StartupPath, "Invoices");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(
                    folder,
                    "Invoice_Order_" + selectedPaymentOrderId + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf"
                );

                InvoicePdfHelper.CreateInvoicePdf(filePath, selectedPaymentOrderId);

                MessageBox.Show("PDF invoice created successfully!");

                Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Message: " + ex.Message +
                    "\n\nType: " + ex.GetType().FullName +
                    "\n\nInner: " + (ex.InnerException != null ? ex.InnerException.Message : "No inner exception")
                );
            }
        }

        private void dgv_payment_history_DoubleClick(object sender, EventArgs e)
        {
            if (dgv_payment_history.CurrentRow == null || dgv_payment_history.CurrentRow.Index == -1)
                return;

            selectedPaymentOrderId = Convert.ToInt32(dgv_payment_history.CurrentRow.Cells["OrderId"].Value);

            LoadPaymentOrderItems(selectedPaymentOrderId);
        }
    }
}