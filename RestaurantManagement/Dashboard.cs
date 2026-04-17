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
        Payment payment_Model = new Payment();
        EmployeeActivity model_Activity = new EmployeeActivity();  
        private List<CartItemModel> cartItems = new List<CartItemModel>();
        private int selectedProductId = 0;
        private decimal selectedProductPrice = 0;
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

            LoadNotifications();
            UpdateNotificationIcon();
            panel_notifications.Visible = false;
            panel_notifications.BringToFront();
            panel_notifications.Left = btn_notifications.Left - 55;
            panel_notifications.Top = btn_notifications.Bottom + 3;

            lbl_fullname.Text = CurrentUser.Username;
            lbl_role.Text = CurrentUser.Role;

            if (CurrentUser.Image != null && CurrentUser.Image.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(CurrentUser.Image))
                {
                    picture_auth.BackgroundImage = Image.FromStream(ms);
                }
            }
            else
            {
                picture_user.BackgroundImage = picture_auth.BackgroundImage;
            }

            LoadUser();
            LoadProducts();
            LoadTables();
            LoadOrders_Products();
            LoadPaymentSearchBy();
            LoadPaymentHistory();
            LoadActivityCNIE();
            LoadActivityLog();

            dgv_activity_log.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_activity_log.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_activity_log.MultiSelect = false;
            dgv_activity_log.ReadOnly = true;
            dgv_activity_log.AllowUserToAddRows = false;

            dgv_payment_history.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_payment_history.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_payment_history.MultiSelect = false;
            dgv_payment_history.ReadOnly = true;
            dgv_payment_history.AllowUserToAddRows = false;

            LoadDashboardStats();
            LoadRecentOrdersDashboard();
            LoadRecentActivitiesDashboard();

            dgv_recent_orders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_recent_orders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_recent_orders.MultiSelect = false;
            dgv_recent_orders.ReadOnly = true;
            dgv_recent_orders.AllowUserToAddRows = false;

            dgv_recent_activity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_recent_activity.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_recent_activity.MultiSelect = false;
            dgv_recent_activity.ReadOnly = true;
            dgv_recent_activity.AllowUserToAddRows = false;

            LoadReportTypes();

            dgv_reports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_reports.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_reports.MultiSelect = false;
            dgv_reports.ReadOnly = true;
            dgv_reports.AllowUserToAddRows = false;

            lbl_report_title.Text = "";
            lbl_report_total.Text = "";

            datefrom_report.Value = DateTime.Today;
            dateto_report.Value = DateTime.Today;

            dateto_report.MinDate = datefrom_report.Value;

            LoadDetailsOrderTableCombos();
            LoadTodayDetailsOrders();

            dgv_detailsorders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_detailsorders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_detailsorders.MultiSelect = false;
            dgv_detailsorders.ReadOnly = true;
            dgv_detailsorders.AllowUserToAddRows = false;

            DisableDetailsOrdersInputs();

            //Timer date and time
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
            // Set current date and time to datepicker
            datereservation.MinDate = DateTime.Now;

            //Tabcontrol
            ActivateButton(btn_dashboard);
            ApplyRolePermissions();

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

        void ApplyRolePermissions()
        {
            string role = CurrentUser.Role?.Trim().ToLower();

            btn_dashboard.Enabled = false;
            btn_orders.Enabled = false;
            tables.Enabled = false;
            btn_menu.Enabled = false;
            btn_staff.Enabled = false;
            btn_payment.Enabled = false;
            btn_history.Enabled = false;
            btn_reports.Enabled = false;

            if (role == "employe")
            {
                btn_dashboard.Enabled = true;
                btn_orders.Enabled = true;
                tables.Enabled = true;
            }
            else if (role == "coocker")
            {
                dash.Visible = false;
                orders.Visible = false;
                table.Visible = false;
                products.Visible = false;
                staff.Visible = false;
                payment.Visible = false; 
                history.Visible = false;
                reports.Visible = false;
                search_dashboard.Enabled = false;
                btn_search_dashboard.Enabled = false;
                coocker.Visible = true;
            }
            else if (role == "admin")
            {
                btn_dashboard.Enabled = true;
                btn_orders.Enabled = true;
                tables.Enabled = true;
                btn_menu.Enabled = true;
                btn_staff.Enabled = true;
                btn_payment.Enabled = true;
                btn_history.Enabled = true;
                btn_reports.Enabled = true;
            }
        }

        //////////////////// Timer date and time
        private void Timer1_Tick(object sender, EventArgs e)
        {
            labelDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        //////////////////// Logout button
        private void btn_logout_Click(object sender, EventArgs e)
        {
            ActivityLogger.Log("Logout", "Auth", null);

            CurrentUser.CNIE = null;
            CurrentUser.Username = null;
            CurrentUser.Role = null;
            CurrentUser.Image = null;

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
            reports.Visible = false;

            panel_notifications.Visible = false;
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
            reports.Visible = false;

            panel_notifications.Visible = false;
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
            reports.Visible = false;

            panel_notifications.Visible = false;
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
            reports.Visible = false;

            panel_notifications.Visible = false;
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
            reports.Visible = false;

            panel_notifications.Visible = false;
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
            reports.Visible = false;

            panel_notifications.Visible = false;
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
            reports.Visible = false;

            panel_notifications.Visible = false;
        }

        private void btn_reports_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);

            reports.Visible = true;
            table.Visible = false;
            history.Visible = false;
            payment.Visible = false;
            staff.Visible = false;
            products.Visible = false;
            orders.Visible = false;
            dash.Visible = false;

            panel_notifications.Visible = false;
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

        void RefreshDashboardStats()
        {
            LoadDashboardStats();
            LoadRecentOrdersDashboard();
            LoadRecentActivitiesDashboard();
            LoadNotifications();
            LoadTodayDetailsOrders();
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
                if (Regex.IsMatch(lastname.Text, @"^[a-zA-Z ]+$"))
                {
                    model_User.LastName = lastname.Text.Trim();
                }
                else
                {
                    MessageBox.Show("LastName must contain only letters");
                    return;
                }
                if (Regex.IsMatch(firstname.Text, @"^[a-zA-Z ]+$"))
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
                            model_User.PasswordHash = PasswordHelper.ComputeHash(password.Text.Trim());
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
                if (Regex.IsMatch(name_products.Text, @"^[a-zA-Z ]+$"))
                {
                    model_Products.Name = name_products.Text;
                }
                else
                {
                    MessageBox.Show("name_products must contain only letters");
                    return;
                }
                if (decimal.TryParse(price_products.Text.Trim(), out decimal price))
                {
                    model_Products.Price = (decimal)price;
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
                        db.SaveChanges();
                        ActivityLogger.Log("Insert", "Products", model_Products.ProductId);
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
                        db.SaveChanges();
                        ActivityLogger.Log("Update", "Products", existingProduct.ProductId);
                    }
                }
                RefreshDashboardStats();

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
                        if (Regex.IsMatch(customername.Text, @"^[a-zA-Z ]+$"))
                        {
                            newReservation.CustomerName = customername.Text.Trim();
                        }
                        else
                        {
                            MessageBox.Show("customername must contain only letters");
                            return;
                        }
                        if (Regex.IsMatch(customerphone.Text, @"^\d+$") && customerphone.Text.Length == 10)
                        {
                            newReservation.CustomerPhone = customerphone.Text.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Phone must contain only numbers and be 10 digits long");
                            return;
                        }
                        newReservation.Status = statusreservation.Text;
                        newReservation.ReservationTime = DateTime.Now;
                        newReservation.Reserveat = datereservation.Value;

                        db.Reservations.Add(newReservation);
                        table.Status = "Reserved";

                        db.SaveChanges();

                        ActivityLogger.Log("Insert", "Reservations", newReservation.ReservationId);

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
                        db.SaveChanges();
                        ActivityLogger.Log("Update", "Reservations", existingReservation.ReservationId);
                    }
                    LoadTables();
                }
                RefreshDashboardStats();

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
                    selectedProductPrice = (Decimal)product.Price;
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
                   tablereserved_orders.SelectedValue,
                   model_Order.OrderId
                );

                DialogResult result = frm.ShowDialog();

                if (result == DialogResult.OK && frm.PaymentCompleted)
                {
                    ClearOrders();
                    LoadOrders_Products();

                    RefreshDashboardStats();
                }
            }
            else
            {
                MessageBox.Show("Please fill in all required fields");
            }
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
                        item.UnitPrice = (Decimal)oi.UnitPrice;
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

        //////////////////// Activity Log
        void LoadActivityCNIE()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                var users = db.Users
                    .Select(u => new
                    {
                        u.CNIE
                    })
                    .OrderBy(u => u.CNIE)
                    .ToList();

                cnie_activity.DataSource = users;
                cnie_activity.DisplayMember = "CNIE";
                cnie_activity.ValueMember = "CNIE";
                cnie_activity.SelectedIndex = -1;
                cnie_activity.Text = "";
            }
        }

        void LoadActivityLog()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                var result = db.EmployeeActivities
                    .Join(
                        db.Users,
                        a => a.CNIE,
                        u => u.CNIE,
                        (a, u) => new
                        {
                            a.ActivityId,
                            a.CNIE,
                            FullName = u.LastName + " " + u.FIrstName,
                            a.Action,
                            a.Entity,
                            a.EntityId,
                            a.CreatedAt
                        }
                    )
                    .OrderByDescending(x => x.ActivityId)
                    .ToList();

                dgv_activity_log.DataSource = result;
            }
        }

        private void btn_search_activity_Click(object sender, EventArgs e)
        {
            string entityText = entity_activity.Text.Trim();

            using (EFDBEntities db = new EFDBEntities())
            {
                var query = db.EmployeeActivities
                    .Join(
                        db.Users,
                        a => a.CNIE,
                        u => u.CNIE,
                        (a, u) => new
                        {
                            a.ActivityId,
                            a.CNIE,
                            FullName = u.LastName + " " + u.FIrstName,
                            a.Action,
                            a.Entity,
                            a.EntityId,
                            a.CreatedAt
                        }
                    );

                if (cnie_activity.SelectedIndex != -1 && cnie_activity.SelectedValue != null)
                {
                    string cnie = cnie_activity.SelectedValue.ToString();
                    query = query.Where(x => x.CNIE == cnie);
                }

                if (!string.IsNullOrWhiteSpace(entityText))
                {
                    query = query.Where(x => x.Entity.Contains(entityText));
                }

                dgv_activity_log.DataSource = query
                    .OrderByDescending(x => x.ActivityId)
                    .ToList();
            }
        }

        private void btn_clear_activity_Click(object sender, EventArgs e)
        {
            cnie_activity.SelectedIndex = -1;
            cnie_activity.Text = "";
            entity_activity.SelectedIndex = -1;

            LoadActivityLog();
        }

        private void dgv_activity_log_DoubleClick(object sender, EventArgs e)
        {
            if (dgv_activity_log.CurrentRow == null || dgv_activity_log.CurrentRow.Index == -1)
                return;

            string cnie = dgv_activity_log.CurrentRow.Cells["CNIE"].Value.ToString();
        }

        //////////////////// Statistics
        void LoadDashboardStats()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                // Total Orders Today
                int totalOrdersToday = db.Orders
                    .Count(o => o.CreatedAt >= today && o.CreatedAt < tomorrow);

                lbl_total_orders_today.ForeColor = Color.FromArgb(52, 152, 219); // Blue
                lbl_total_orders_today.Text = totalOrdersToday.ToString();

                // Total Revenue Today
                double totalRevenueToday = db.Payments
                    .Where(p => p.PaidAt >= today && p.PaidAt < tomorrow)
                    .Select(p => (double?)p.Amount)
                    .Sum() ?? 0;

                lbl_total_revenue_today.ForeColor = Color.FromArgb(46, 204, 113); // Green
                lbl_total_revenue_today.Text = totalRevenueToday.ToString("0.00") + " MAD";

                // Tables Status
                int freeTables = db.RestaurantTables.Count(t => t.Status == "Free");
                int occupiedTables = db.RestaurantTables.Count(t => t.Status == "Occupied");
                int reservedTables = db.RestaurantTables.Count(t => t.Status == "Reserved");

                lbl_tables_free.ForeColor = Color.FromArgb(46, 204, 113);
                lbl_tables_free.Text = "Free : " + freeTables;
                lbl_tables_occupied.ForeColor = Color.FromArgb(231, 76, 60);
                lbl_tables_occupied.Text = "Occupied : " + occupiedTables;
                lbl_tables_reserved.ForeColor = Color.FromArgb(241, 196, 15);
                lbl_tables_reserved.Text = "Reserved : " + reservedTables;

                // DineIn vs TakeAway Today
                int dineInToday = db.Orders.Count(o =>
                    o.CreatedAt >= today && o.CreatedAt < tomorrow && o.OrderType == "DineIn");

                int takeAwayToday = db.Orders.Count(o =>
                    o.CreatedAt >= today && o.CreatedAt < tomorrow && o.OrderType == "TakeAway");

                lbl_dinein_today.ForeColor = Color.FromArgb(39, 174, 96);
                lbl_dinein_today.Text = dineInToday.ToString();
                lbl_takeaway_today.ForeColor = Color.FromArgb(230, 126, 34);
                lbl_takeaway_today.Text = takeAwayToday.ToString();

                // Notifications count
                lbl_notifications_count.ForeColor = Color.FromArgb(231, 76, 60); // Red
                DateTime today_order = DateTime.Today;
                DateTime tomorrow_order = today.AddDays(1);

                int notificationsCount = db.Orders
                    .Count(o => o.CreatedAt.HasValue
                             && o.CreatedAt.Value >= today_order
                             && o.CreatedAt.Value < tomorrow_order
                             && (o.Status == "WaitList" || o.Status == "Confirmed"));

                lbl_notifications_count.Text = notificationsCount.ToString();
            }
        }

        void LoadRecentOrdersDashboard()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                var recentOrders = db.Orders
                    .OrderByDescending(o => o.OrderId)
                    .Take(10)
                    .Select(o => new
                    {
                        o.OrderId,
                        o.CustomerName,
                        o.CustomerPhone,
                        o.OrderType,
                        o.Status,
                        o.Price,
                        o.CreatedAt
                    })
                    .ToList();

                dgv_recent_orders.DataSource = recentOrders;
            }

            dgv_recent_orders.ClearSelection();
        }

        void LoadRecentActivitiesDashboard()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                var recentActivities = db.EmployeeActivities
                    .Join(
                        db.Users,
                        a => a.CNIE,
                        u => u.CNIE,
                        (a, u) => new
                        {
                            a.ActivityId,
                            a.CNIE,
                            FullName = u.LastName + " " + u.FIrstName,
                            a.Action,
                            a.Entity,
                            a.EntityId,
                            a.CreatedAt
                        }
                    )
                    .OrderByDescending(x => x.ActivityId)
                    .Take(10)
                    .ToList();

                dgv_recent_activity.DataSource = recentActivities;
            }

            dgv_recent_activity.ClearSelection();
        }

        private void btn_search_dashboard_Click(object sender, EventArgs e)
        {
            string value = search_dashboard.Text.Trim();

            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show("Enter something to search");
                return;
            }

            using (EFDBEntities db = new EFDBEntities())
            {
                int orderId;
                bool isNumber = int.TryParse(value, out orderId);

                var orders = db.Orders
                    .Where(o =>
                        (isNumber && o.OrderId == orderId) || // OrderId
                        o.CustomerName.Contains(value) ||     // Name
                        o.CustomerPhone.Contains(value) ||    // Phone
                        o.OrderType.Contains(value) ||        // Type
                        o.Status.Contains(value)              // Status
                    )
                    .Select(o => new
                    {
                        o.OrderId,
                        o.CustomerName,
                        o.CustomerPhone,
                        o.OrderType,
                        o.Status,
                        o.Price,
                        o.CreatedAt
                    })
                    .ToList();

                dgv_recent_orders.DataSource = orders;
            }
        }

        private void search_dashboard_TextChanged(object sender, EventArgs e)
        {
            btn_search_dashboard.PerformClick();
        }

        //////////////////// Reports
        void LoadReportsStats()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                // Total Orders All Time
                int totalOrders = db.Orders.Count();
                lbl_total_orders_today.Text = totalOrders.ToString();

                // Total Revenue All Time
                decimal totalRevenue = db.Payments
                    .Select(p => (decimal?)p.Amount)
                    .Sum() ?? 0;

                lbl_total_revenue_today.Text = totalRevenue.ToString("0.00") + " MAD";

                // Tables Status
                int freeTables = db.RestaurantTables.Count(t => t.Status == "Free");
                int occupiedTables = db.RestaurantTables.Count(t => t.Status == "Occupied");
                int reservedTables = db.RestaurantTables.Count(t => t.Status == "Reserved");

                lbl_tables_free.Text = "Free : " + freeTables;
                lbl_tables_occupied.Text = "Occupied : " + occupiedTables;
                lbl_tables_reserved.Text = "Reserved : " + reservedTables;

                // DineIn vs TakeAway All Time
                int dineInCount = db.Orders.Count(o => o.OrderType == "DineIn");
                int takeAwayCount = db.Orders.Count(o => o.OrderType == "TakeAway");

                lbl_dinein_today.Text = dineInCount.ToString();
                lbl_takeaway_today.Text = takeAwayCount.ToString();

                // Notifications
                int notificationsCount = db.Orders.Count(o => o.Status == "WaitList");
                lbl_notifications_count.Text = notificationsCount.ToString();
            }
        }

        void LoadReportTypes()
        {
            report_type.Items.Clear();

            report_type.Items.Add("Sales by Day");
            report_type.Items.Add("Sales by Month");
            report_type.Items.Add("Total Revenue");
            report_type.Items.Add("Most Sold Products");
            report_type.Items.Add("Orders by Employee");
            report_type.Items.Add("Tables Usage");
            report_type.Items.Add("DineIn vs TakeAway");

            report_type.SelectedIndex = -1;
        }

        private void datefrom_report_ValueChanged(object sender, EventArgs e)
        {
            dateto_report.MinDate = datefrom_report.Value;

            if (dateto_report.Value < datefrom_report.Value)
            {
                dateto_report.Value = datefrom_report.Value;
            }
        }

        private void btn_generate_report_Click(object sender, EventArgs e)
        {
            if (report_type.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a report type");
                return;
            }

            string selectedReport = report_type.Text;
            DateTime fromDate = datefrom_report.Value.Date;
            DateTime toDate = dateto_report.Value.Date.AddDays(1);

            using (EFDBEntities db = new EFDBEntities())
            {
                // 1) Sales by Day
                if (selectedReport == "Sales by Day")
                {
                    var rawData = db.Payments
                        .Where(p => p.PaidAt.HasValue &&
                                    p.PaidAt.Value >= fromDate &&
                                    p.PaidAt.Value < toDate)
                        .GroupBy(p => new
                        {
                            Year = p.PaidAt.Value.Year,
                            Month = p.PaidAt.Value.Month,
                            Day = p.PaidAt.Value.Day
                        })
                        .Select(g => new
                        {
                            g.Key.Year,
                            g.Key.Month,
                            g.Key.Day,
                            TotalSales = g.Sum(x => x.Amount),
                            PaymentsCount = g.Count()
                        })
                        .ToList();

                    var data = rawData
                        .Select(x => new
                        {
                            Date = new DateTime(x.Year, x.Month, x.Day),
                            x.TotalSales,
                            x.PaymentsCount
                        })
                        .OrderBy(x => x.Date)
                        .ToList();

                    dgv_reports.DataSource = data;
                    lbl_report_title.Text = "Sales by Day";
                    lbl_report_total.Text = "Total: " + data.Sum(x => x.TotalSales).GetValueOrDefault().ToString("0.00") + " MAD";
                }

                // 2) Sales by Month
                else if (selectedReport == "Sales by Month")
                {
                    var data = db.Payments
                        .Where(p => p.PaidAt.HasValue && p.PaidAt.Value >= fromDate && p.PaidAt.Value < toDate)
                        .GroupBy(p => new
                        {
                            Year = p.PaidAt.Value.Year,
                            Month = p.PaidAt.Value.Month
                        })
                        .Select(g => new
                        {
                            g.Key.Year,
                            g.Key.Month,
                            TotalSales = g.Sum(x => x.Amount),
                            PaymentsCount = g.Count()
                        })
                        .OrderBy(x => x.Year)
                        .ThenBy(x => x.Month)
                        .ToList();

                    dgv_reports.DataSource = data;
                    lbl_report_title.Text = "Sales by Month";
                    decimal totalSales = data.Sum(x => x.TotalSales) ?? 0m;
                    lbl_report_total.Text = "Total: " + totalSales.ToString("0.00") + " MAD";
                }

                // 3) Total Revenue All Time
                else if (selectedReport == "Total Revenue")
                {
                    var data = db.Payments
                        .Where(p => p.PaidAt.HasValue &&
                                    p.PaidAt.Value >= fromDate &&
                                    p.PaidAt.Value < toDate)
                        .Select(p => new
                        {
                            p.PaymentId,
                            p.OrderId,
                            p.Method,
                            p.Amount,
                            p.PaidAt
                        })
                        .OrderByDescending(x => x.PaymentId)
                        .ToList();

                    decimal totalRevenue = data.Sum(x => (decimal)x.Amount);

                    dgv_reports.DataSource = data;
                    lbl_report_title.Text = "Total Revenue";
                    lbl_report_total.Text = "Total: " + totalRevenue.ToString("0.00") + " MAD";
                }

                // 4) Most Sold Products
                else if (selectedReport == "Most Sold Products")
                {
                    var data = db.OrderItems
                        .Join(
                            db.Products,
                            oi => oi.ProductId,
                            p => p.ProductId,
                            (oi, p) => new { oi, p }
                        )
                        .GroupBy(x => new
                        {
                            x.oi.ProductId,
                            x.p.Name
                        })
                        .Select(g => new
                        {
                            ProductId = g.Key.ProductId,
                            ProductName = g.Key.Name,
                            TotalQuantitySold = g.Sum(x => x.oi.Quantity),
                            TotalRevenue = g.Sum(x => x.oi.Quantity * x.oi.UnitPrice)
                        })
                        .OrderByDescending(x => x.TotalQuantitySold)
                        .ToList();

                    dgv_reports.DataSource = data;
                    lbl_report_title.Text = "Most Sold Products";
                    lbl_report_total.Text = "Products Count: " + data.Count;
                }

                // 5) Orders by Employee
                else if (selectedReport == "Orders by Employee")
                {
                    var data = db.EmployeeActivities
                        .Where(a => a.Entity == "Orders" && a.Action == "Insert")
                        .Join(
                            db.Users,
                            a => a.CNIE,
                            u => u.CNIE,
                            (a, u) => new
                            {
                                a.CNIE,
                                FullName = u.LastName + " " + u.FIrstName,
                                a.EntityId
                            }
                        )
                        .GroupBy(x => new
                        {
                            x.CNIE,
                            x.FullName
                        })
                        .Select(g => new
                        {
                            g.Key.CNIE,
                            g.Key.FullName,
                            OrdersCount = g.Count()
                        })
                        .OrderByDescending(x => x.OrdersCount)
                        .ToList();

                    dgv_reports.DataSource = data;
                    lbl_report_title.Text = "Orders by Employee";
                    lbl_report_total.Text = "Employees: " + data.Count;
                }

                // 6) Tables Usage
                else if (selectedReport == "Tables Usage")
                {
                    var data = db.Orders
                        .Where(o => o.TableId.HasValue)
                        .Join(
                            db.RestaurantTables,
                            o => o.TableId.Value,
                            t => t.TableId,
                            (o, t) => new
                            {
                                t.TableId,
                                t.TableNumber,
                                o.OrderId
                            }
                        )
                        .GroupBy(x => new
                        {
                            x.TableId,
                            x.TableNumber
                        })
                        .Select(g => new
                        {
                            g.Key.TableId,
                            g.Key.TableNumber,
                            OrdersCount = g.Count()
                        })
                        .OrderByDescending(x => x.OrdersCount)
                        .ToList();

                    dgv_reports.DataSource = data;
                    lbl_report_title.Text = "Tables Usage";
                    lbl_report_total.Text = "Used Tables: " + data.Count;
                }

                // 7) DineIn vs TakeAway
                else if (selectedReport == "DineIn vs TakeAway")
                {
                    int dineInCount = db.Orders.Count(o => o.OrderType == "DineIn");
                    int takeAwayCount = db.Orders.Count(o => o.OrderType == "TakeAway");

                    decimal dineInRevenue = db.Orders
                        .Where(o => o.OrderType == "DineIn")
                        .Select(o => (decimal?)o.Price)
                        .Sum() ?? 0;

                    decimal takeAwayRevenue = db.Orders
                        .Where(o => o.OrderType == "TakeAway")
                        .Select(o => (decimal?)o.Price)
                        .Sum() ?? 0;

                    var data = new[]
                    {
                new
                {
                    OrderType = "DineIn",
                    OrdersCount = dineInCount,
                    Revenue = dineInRevenue
                },
                new
                {
                    OrderType = "TakeAway",
                    OrdersCount = takeAwayCount,
                    Revenue = takeAwayRevenue
                }
            }.ToList();

                    dgv_reports.DataSource = data;
                    lbl_report_title.Text = "DineIn vs TakeAway";
                    lbl_report_total.Text = "DineIn: " + dineInCount + " | TakeAway: " + takeAwayCount;
                }
            }
        }

        private void btn_clear_report_Click(object sender, EventArgs e)
        {
            report_type.SelectedIndex = -1;
            lbl_report_title.Text = "";
            lbl_report_total.Text = "";
            dgv_reports.DataSource = null;
        }

        //////////////////// Notifications

        void UpdateNotificationIcon()
        {
            int notifCount = 0;
            int.TryParse(lbl_notifications_count.Text, out notifCount);

            if (notifCount > 0)
            {
                btn_notifications.BackgroundImage = Properties.Resources.notification__1_;
            }
            else
            {
                btn_notifications.BackgroundImage = Properties.Resources.bell__1_;
            }
        }

        void LoadNotifications()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                var data = db.Orders
                    .Where(o => o.CreatedAt.HasValue
                             && o.CreatedAt.Value >= today
                             && o.CreatedAt.Value < tomorrow
                             && (o.Status == "WaitList" || o.Status == "Confirmed"))
                    .OrderByDescending(o => o.CreatedAt)
                    .ThenByDescending(o => o.OrderId)
                    .Select(o => new
                    {
                        o.OrderId,
                        o.CustomerName,
                        o.OrderType,
                        o.Status,
                        o.CreatedAt
                    })
                    .ToList();

                dgv_notifications.DataSource = null;
                dgv_notifications.DataSource = data;

                lbl_notifications_count.Text = data.Count.ToString();
            }

            UpdateNotificationIcon();
        }

        private void btn_notifications_Click(object sender, EventArgs e)
        {
            panel_notifications.Parent = this;
            panel_notifications.Visible = !panel_notifications.Visible;
            panel_notifications.BringToFront();
        }

        private Order selectedDetailsOrder = null;
        private bool isLoadingDetailsOrder = false;

        //////////////////// cook details   

        void DisableDetailsOrdersInputs()
        {
            ordertype_details.Enabled = false;
            customername_details.Enabled = false;
            customerphone_details.Enabled = false;
            table_details.Enabled = false;
            tablereserved_details.Enabled = false;
            btn_cart_details.Enabled = true;   
            status_details.Enabled = true;    
        }

        void ClearDetailsOrders()
        {
            isLoadingDetailsOrder = true;

            ordertype_details.SelectedIndex = -1;
            customername_details.Text = "";
            customerphone_details.Text = "";
            table_details.DataSource = null;
            tablereserved_details.DataSource = null;
            status_details.SelectedIndex = -1;

            cartItems.Clear();
            RefreshCartForm();
            lbl_cart_count.Text = "0";

            selectedDetailsOrder = null;

            DisableDetailsOrdersInputs();

            isLoadingDetailsOrder = false;
        }

        void LoadDetailsOrderTableCombos(int? selectedTableId = null)
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
                                Display = "Table " + t.TableNumber + " Capacity " + t.Capacity + " (" + t.Status + ")"
                            })
                            .FirstOrDefault();

                        if (currentTable != null)
                        {
                            freeTables.Add(currentTable);
                        }
                    }
                }

                table_details.DataSource = null;
                table_details.DataSource = freeTables;
                table_details.DisplayMember = "Display";
                table_details.ValueMember = "TableId";
                table_details.SelectedIndex = -1;

                tablereserved_details.DataSource = null;
                tablereserved_details.DataSource = reservedTables;
                tablereserved_details.DisplayMember = "Display";
                tablereserved_details.ValueMember = "TableId";
                tablereserved_details.SelectedIndex = -1;
            }
        }

        void LoadTodayDetailsOrders()
        {
            using (EFDBEntities db = new EFDBEntities())
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                var data = db.Orders
                    .Where(o => o.CreatedAt.HasValue
                             && o.CreatedAt.Value >= today
                             && o.CreatedAt.Value < tomorrow
                             && (o.Status == "WaitList"
                              || o.Status == "Confirmed"
                              || o.Status == "Prepared"
                              || o.Status == "Preparing"))
                    .OrderBy(o => o.CreatedAt)
                    .ThenBy(o => o.OrderId)
                    .Select(o => new
                    {
                        o.OrderId,
                        o.OrderType,
                        o.CustomerName,
                        o.CustomerPhone,
                        o.Status,
                        o.TableId,
                        o.CreatedAt,
                        o.Price
                    })
                    .ToList();

                dgv_detailsorders.DataSource = null;
                dgv_detailsorders.DataSource = data;
            }

            dgv_detailsorders.ClearSelection();
        }

        private void dgv_detailsorders_DoubleClick(object sender, EventArgs e)
        {
            if (dgv_detailsorders.CurrentRow == null || dgv_detailsorders.CurrentRow.Index == -1)
                return;

            int orderId = Convert.ToInt32(dgv_detailsorders.CurrentRow.Cells["OrderId"].Value);

            using (var db = new EFDBEntities())
            {
                var order = db.Orders.Find(orderId);

                if (order == null)
                {
                    MessageBox.Show("Order not found");
                    return;
                }

                isLoadingDetailsOrder = true;
                selectedDetailsOrder = order;

                DisableDetailsOrdersInputs();

                ordertype_details.Text = order.OrderType;
                customername_details.Text = order.CustomerName;
                customerphone_details.Text = order.CustomerPhone;
                status_details.Text = order.Status;

                if (order.OrderType == "DineIn")
                {
                    LoadDetailsOrderTableCombos(order.TableId);

                    if (order.TableId != null)
                    {
                        bool isReservedTable = db.Reservations
                            .Any(r => r.TableId == order.TableId.Value && r.Status == "Reserved");

                        if (isReservedTable)
                        {
                            tablereserved_details.SelectedValue = order.TableId.Value;
                            table_details.SelectedIndex = -1;
                        }
                        else
                        {
                            table_details.SelectedValue = order.TableId.Value;
                            tablereserved_details.SelectedIndex = -1;
                        }
                    }
                    else
                    {
                        table_details.SelectedIndex = -1;
                        tablereserved_details.SelectedIndex = -1;
                    }
                }
                else
                {
                    table_details.DataSource = null;
                    tablereserved_details.DataSource = null;
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
                        item.UnitPrice = (Decimal)oi.UnitPrice;
                        item.Quantity = oi.Quantity;
                        item.Image = product.Image;

                        cartItems.Add(item);
                    }
                }

                isLoadingDetailsOrder = false;
            }

            UpdateCartCount();
            RefreshCartFormReadOnly();
        }

        private void RefreshCartFormReadOnly()
        {
            if (cartForm == null || cartForm.IsDisposed)
            {
                cartForm = new CartProducts();
            }

            cartForm.LoadCartProductsReadOnly(cartItems);
        }

        private void btn_cart_details_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("No products in this order");
                return;
            }

            if (cartForm == null || cartForm.IsDisposed)
            {
                cartForm = new CartProducts();
            }

            cartForm.LoadCartProductsReadOnly(cartItems);
            cartForm.Show();
            cartForm.BringToFront();
        }

        private void btn_valide_orderdetails_Click(object sender, EventArgs e)
        {
            if (selectedDetailsOrder == null)
            {
                MessageBox.Show("Please double click an order first");
                return;
            }

            if (status_details.Text == "")
            {
                MessageBox.Show("Please select status");
                return;
            }

            using (EFDBEntities db = new EFDBEntities())
            {
                var order = db.Orders.Find(selectedDetailsOrder.OrderId);

                if (order == null)
                {
                    MessageBox.Show("Order not found");
                    return;
                }

                order.Status = status_details.Text;

                db.SaveChanges();

                ActivityLogger.Log("Update Status", "Orders", order.OrderId);

                if (order.Status == "Served" && order.TableId != null)
                {
                    var table = db.RestaurantTables.Find(order.TableId.Value);
                    if (table != null)
                    {
                        table.Status = "Free";
                        db.SaveChanges();
                    }
                }
            }

            MessageBox.Show("Order status updated successfully!");

            ClearDetailsOrders();
            LoadDetailsOrderTableCombos();
            LoadTodayDetailsOrders();
            RefreshDashboardStats();
            LoadNotifications();
        }

        private void btn_clear_orderdetails_Click(object sender, EventArgs e)
        {
            ClearDetailsOrders();
        }
    }
}