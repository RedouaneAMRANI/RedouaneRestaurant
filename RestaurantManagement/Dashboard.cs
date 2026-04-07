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
using System.Threading.Tasks;
using System.Windows.Forms;

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
            LoadOrders();

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
                model_User.LastName = lastname.Text.Trim();
                model_User.FIrstName = firstname.Text.Trim();
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

        private void btn_save_products_Click(object sender, EventArgs e)
        {
            if (name_products.Text != "" && price_products.Text != "" && categories_products.Text != "" && status_products.Text != "")
            {
                model_Products.Name = name_products.Text.Trim();
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
                        model_Products = product; // ⭐ مهم بزاف

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
                        newReservation.CustomerName = customername.Text.Trim();
                        newReservation.CustomerPhone = customerphone.Text.Trim();
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
                    LoadOrders();
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
            PaymentForm paymentForm = new PaymentForm();
            paymentForm.Show();
        }

        //////////////////// Orders

        void ClearOrders()
        {
            customername_orders.Text = customerphone_orders.Text = "";
            categories_orders.SelectedIndex = -1;
            ordertype_orders.SelectedIndex = -1;
            table_orders.SelectedIndex = -1;
            tablereserved_orders.SelectedIndex = -1;
            product_orders.SelectedIndex = -1;
            status_orders.SelectedIndex = -1;
            quantite_product.Value = 1;
            model_Order = new Order();
            model_OrderItem = new OrderItem();
            btn_save_order.Text = "Save";
            LoadOrders();
        }

        private void btn_clear_order_Click(object sender, EventArgs e)
        {
            ClearOrders();
        }

        void LoadOrders()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                    var freeTables = db.RestaurantTables
                                       .Where(t => t.Status == "Free")
                                       .ToList()
                                       .Select(t => new
                                       {
                                           t.TableId,
                                           Display = "Table " + t.TableNumber + " Capacity " + t.Capacity
                                       })
                                       .ToList();

                    table_orders.DataSource = freeTables;
                    table_orders.DisplayMember = "Display";
                    table_orders.ValueMember = "TableId";
                    table_orders.SelectedIndex = -1;

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
                           Display = "Table " + x.TableNumber + " Capacity " + x.Capacity
                       })
                       .ToList();

                tablereserved_orders.DataSource = reservedTables;
                tablereserved_orders.DisplayMember = "Display";
                tablereserved_orders.ValueMember = "TableId";
                tablereserved_orders.SelectedIndex = -1;
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
            }
        }

        private void ordertype_orders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ordertype_orders.Text == "TakeAway")
            {
                table_orders.Enabled = false;
                tablereserved_orders.Enabled = false;
                tablereserved_orders.SelectedIndex = -1;
                table_orders.SelectedIndex = -1;
            }
            else
            {
                table_orders.Enabled = true;
                tablereserved_orders.Enabled = true;
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

        private void btn_save_order_Click(object sender, EventArgs e)
        {
            if (ordertype_orders.Text != "" && product_orders.Text != "" && status_orders.Text != "")
            {
                using (EFDBEntities db = new EFDBEntities())
                {
                    if (model_Order.OrderId == 0)
                    {
                        Order newOrder = new Order();

                        newOrder.OrderType = ordertype_orders.Text;
                        newOrder.Status = status_orders.Text;
                        newOrder.CustomerName = customername_orders.Text.Trim();
                        newOrder.CustomerPhone = customerphone_orders.Text.Trim();

                        if (ordertype_orders.Text == "DineIn")
                        {
                            if (table_orders.SelectedValue != null)
                            {
                                int tableId = Convert.ToInt32(table_orders.SelectedValue);
                                newOrder.TableId = tableId;

                                var table = db.RestaurantTables.Find(tableId);
                                if (table != null)
                                {
                                    table.Status = "Occupied";
                                }
                            }
                            else if (tablereserved_orders.SelectedValue != null)
                            {
                                int tableId = Convert.ToInt32(tablereserved_orders.SelectedValue);
                                newOrder.TableId = tableId;

                                var reservation = db.Reservations
                                                    .FirstOrDefault(r => r.TableId == tableId && r.Status == "Reserved");

                                if (reservation != null)
                                {
                                    newOrder.CustomerName = reservation.CustomerName;
                                    newOrder.CustomerPhone = reservation.CustomerPhone;

                                    reservation.Status = "Completed";

                                    var table = db.RestaurantTables.Find(tableId);
                                    if (table != null)
                                    {
                                        table.Status = "Occupied";
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please select a table for Dine-In orders.");
                                return;
                            }
                        }
                        else if (ordertype_orders.Text == "TakeAway" || ordertype_orders.Text == "Online")
                        {
                            if (customername_orders.Text == "" || customerphone_orders.Text == "")
                            {
                                MessageBox.Show("Please enter customer name and phone.");
                                return;
                            }

                            newOrder.CustomerName = customername_orders.Text.Trim();
                            newOrder.CustomerPhone = customerphone_orders.Text.Trim();
                            newOrder.TableId = null;
                        }

                        db.Orders.Add(newOrder);
                    }
                    else
                    {
                        var existingOrder = db.Orders.Find(model_Order.OrderId);

                        if (existingOrder == null) return;

                        if (MessageBox.Show("Are you sure to update?", "Message", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;

                        existingOrder.OrderType = ordertype_orders.Text;
                        existingOrder.Status = status_orders.Text;
                        existingOrder.CustomerName = customername_orders.Text.Trim();
                        existingOrder.CustomerPhone = customerphone_orders.Text.Trim();

                        if (ordertype_orders.Text == "DineIn")
                        {
                            if (table_orders.SelectedValue != null)
                            {
                                int tableId = Convert.ToInt32(table_orders.SelectedValue);
                                existingOrder.TableId = tableId;

                                var table = db.RestaurantTables.Find(tableId);
                                if (table != null)
                                {
                                    table.Status = "Occupied";
                                }
                            }
                            else if (tablereserved_orders.SelectedValue != null)
                            {
                                int tableId = Convert.ToInt32(tablereserved_orders.SelectedValue);
                                existingOrder.TableId = tableId;

                                var reservation = db.Reservations
                                                    .FirstOrDefault(r => r.TableId == tableId && r.Status == "Reserved");

                                if (reservation != null)
                                {
                                    existingOrder.CustomerName = reservation.CustomerName;
                                    existingOrder.CustomerPhone = reservation.CustomerPhone;

                                    reservation.Status = "Completed";

                                    var table = db.RestaurantTables.Find(tableId);
                                    if (table != null)
                                    {
                                        table.Status = "Occupied";
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please select a table for Dine-In orders.");
                                return;
                            }
                        }
                        else if (ordertype_orders.Text == "TakeAway" || ordertype_orders.Text == "Online")
                        {
                            if (customername_orders.Text == "" || customerphone_orders.Text == "")
                            {
                                MessageBox.Show("Please enter customer name and phone.");
                                return;
                            }

                            existingOrder.CustomerName = customername_orders.Text.Trim();
                            existingOrder.CustomerPhone = customerphone_orders.Text.Trim();
                            existingOrder.TableId = null;
                        }
                    }

                    db.SaveChanges();
                }

                ClearOrders();
                MessageBox.Show("Submitted successfully!");
            }
            else
            {
                MessageBox.Show("Please fill in all required fields");
            }
        }
    }
}