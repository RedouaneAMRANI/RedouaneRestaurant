using DevComponents.DotNetBar;
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

            //Timer date and time
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

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
            btn_save_restable.Text = "Save";
            LoadTables();
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
                    Panel pnl = this.Controls.Find("Table" + table.TableNumber, true)
                                             .FirstOrDefault() as Panel;

                    if (pnl != null)
                    {
                        switch (table.Status)
                        {
                            case "Free":
                                pnl.BackColor = Color.Green;
                                break;

                            case "Occupied":
                                pnl.BackColor = Color.Red;
                                break;

                            case "Reserved":
                                pnl.BackColor = Color.Orange;
                                break;
                        }

                        foreach (Control ctrl in pnl.Controls)
                        {
                            if (ctrl is Label lbl)
                            {
                                if (lbl.Text.Contains("Capacity"))
                                {
                                    lbl.Text = "Capacity : " + table.Capacity;
                                }
                                else if (lbl.Text.Contains("Status") || lbl.Text.Contains("label20"))
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
            if (tablenumber.Text != "" && customername.Text != "" && customerphone.Text != "" && statusreservation.Text != "")
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

                        // 🔥 تحديث حالة الطاولة
                        table.Status = statusreservation.Text == "Reserved" ? "Occupied" : "Free";
                    }
                    db.SaveChanges();
                }

                ClearTables();
                MessageBox.Show("Submitted successfully!");
            }
            else
            {
                MessageBox.Show("Please fill in all fields");
            }
        }

        private void btn_search_restable_Click(object sender, EventArgs e)
        {

        }

        private void btn_add_order_Click(object sender, EventArgs e)
        {
            PaymentForm paymentForm = new PaymentForm();
            paymentForm.Show();
        }
    }
}