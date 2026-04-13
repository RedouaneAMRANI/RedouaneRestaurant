using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;

namespace RestaurantManagement
{
    public partial class PaymentForm : Form
    {
        private int _savedOrderId = 0;
        private double _totalAmount = 0;
        private bool _isPaid = false;

        private List<CartItemModel> _cartItems;
        private string _orderType;
        private string _status;
        private string _customerName;
        private string _customerPhone;
        private object _tableId;
        private object _reservedTableId;

        public bool PaymentCompleted { get; private set; } = false;
        private int _editingOrderId = 0;

        public PaymentForm(
            List<CartItemModel> cartItems,
            string orderType,
            string status,
            string customerName,
            string customerPhone,
            object tableId,
            object reservedTableId,
            int orderId = 0)
        {
            InitializeComponent();

            _cartItems = cartItems.Select(x => new CartItemModel
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity,
                Image = x.Image
            }).ToList();

            _orderType = orderType;
            _status = status;
            _customerName = customerName;
            _customerPhone = customerPhone;
            _tableId = tableId;
            _reservedTableId = reservedTableId;
            _editingOrderId = orderId;
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            LoadInvoicePreview();
        }

        private void LoadInvoicePreview()
        {
            lbl_orderid.Text = "Not saved yet";
            lbl_status.Text = _status;
            lbl_createdat.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            lbl_source.Text = "Desktop";

            if (_orderType == "DineIn")
            {
                if (_tableId != null)
                {
                    lbl_table_type.Text = "Table / DineIn";
                }
                else if (_reservedTableId != null)
                {
                    lbl_table_type.Text = "Reserved Table / DineIn";
                }
                else
                {
                    lbl_table_type.Text = "DineIn";
                }
            }
            else
            {
                lbl_table_type.Text = _orderType;
            }

            flowLayoutPanel_items.Controls.Clear();
            _totalAmount = 0;

            foreach (var item in _cartItems)
            {
                Label lbl = new Label();
                lbl.AutoSize = false;
                lbl.Width = 330;
                lbl.Height = 28;
                lbl.Text = item.ProductName + "   x" + item.Quantity +
                           "   " + item.UnitPrice.ToString("0,00") +
                           " MAD   =   " + item.Total.ToString("0,00") + " MAD";

                flowLayoutPanel_items.Controls.Add(lbl);

                _totalAmount += item.Total;
            }

            lbl_totalamount.Text = _totalAmount.ToString("0,00") + " MAD";
        }

        private void btn_paynow_Click(object sender, EventArgs e)
        {
            string method = "";

            if (radio_cash.Checked)
                method = "Cash";
            else if (radio_card.Checked)
                method = "Card";
            else
            {
                MessageBox.Show("Select payment method");
                return;
            }

            using (EFDBEntities db = new EFDBEntities())
            {
                Order order;
                bool isNewOrder = (_editingOrderId == 0);

                // 1) ORDER
                if (isNewOrder)
                {
                    order = new Order();
                    db.Orders.Add(order);
                }
                else
                {
                    order = db.Orders.Find(_editingOrderId);

                    if (order == null)
                    {
                        MessageBox.Show("Order not found");
                        return;
                    }
                }

                order.OrderType = _orderType;
                order.Status = _status;

                if (!string.IsNullOrWhiteSpace(_customerName))
                {
                    if (Regex.IsMatch(_customerName, @"^[a-zA-Z0-9]+$"))
                    {
                        order.CustomerName = _customerName.Trim();
                    }
                    else
                    {
                        MessageBox.Show("Customer name must contain only letters and numbers");
                        return;
                    }
                }
                else
                {
                    order.CustomerName = _customerName;
                }

                order.CustomerPhone = _customerPhone;
                order.CreatedAt = order.CreatedAt ?? DateTime.Now;
                order.Price = (decimal)_cartItems.Sum(x => x.Total);

                if (_orderType == "DineIn")
                {
                    if (_tableId != null)
                    {
                        int tableId = Convert.ToInt32(_tableId);
                        order.TableId = tableId;

                        var table = db.RestaurantTables.Find(tableId);
                        if (table != null)
                        {
                            table.Status = "Occupied";
                        }
                    }
                    else if (_reservedTableId != null)
                    {
                        int tableId = Convert.ToInt32(_reservedTableId);
                        order.TableId = tableId;

                        var reservation = db.Reservations
                            .FirstOrDefault(r => r.TableId == tableId && r.Status == "Reserved");

                        if (reservation != null)
                        {
                            order.CustomerName = reservation.CustomerName;
                            order.CustomerPhone = reservation.CustomerPhone;
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
                else if (_orderType == "TakeAway" || _orderType == "Online")
                {
                    if (string.IsNullOrWhiteSpace(_customerName) || string.IsNullOrWhiteSpace(_customerPhone))
                    {
                        MessageBox.Show("Please enter customer name and phone.");
                        return;
                    }

                    order.TableId = null;
                }

                db.SaveChanges();

                if (isNewOrder)
                    ActivityLogger.Log("Insert", "Orders", order.OrderId);
                else
                    ActivityLogger.Log("Update", "Orders", order.OrderId);

                if (!isNewOrder)
                {
                    var oldItems = db.OrderItems.Where(x => x.OrderId == order.OrderId).ToList();
                    foreach (var oldItem in oldItems)
                    {
                        db.OrderItems.Remove(oldItem);
                    }
                    db.SaveChanges();
                }

                foreach (var item in _cartItems)
                {
                    OrderItem oi = new OrderItem();
                    oi.OrderId = order.OrderId;
                    oi.ProductId = item.ProductId;
                    oi.Quantity = item.Quantity;
                    oi.UnitPrice = (decimal)item.UnitPrice;

                    db.OrderItems.Add(oi);
                }

                db.SaveChanges();

                var existingPayment = db.Payments.FirstOrDefault(p => p.OrderId == order.OrderId);
                int? paymentIdForLog = null;

                if (existingPayment == null)
                {
                    Payment pay = new Payment();
                    pay.OrderId = order.OrderId;
                    pay.Method = method;
                    pay.Amount = (decimal)_cartItems.Sum(x => x.Total);
                    pay.PaidAt = DateTime.Now;

                    db.Payments.Add(pay);
                    db.SaveChanges();

                    paymentIdForLog = pay.PaymentId;

                    ActivityLogger.Log("Add Payment", "Payments", pay.PaymentId);
                }
                else
                {
                    existingPayment.Method = method;
                    existingPayment.Amount = (decimal)_cartItems.Sum(x => x.Total);
                    existingPayment.PaidAt = DateTime.Now;

                    db.SaveChanges();

                    paymentIdForLog = existingPayment.PaymentId;

                    ActivityLogger.Log("Update Payment", "Payments", existingPayment.PaymentId);
                }

                _savedOrderId = order.OrderId;
                _isPaid = true;
                PaymentCompleted = true;

                lbl_orderid.Text = _savedOrderId.ToString();

                MessageBox.Show(isNewOrder
                    ? "Order + Payment saved successfully!"
                    : "Order + Payment updated successfully!");

                string folder = Path.Combine(Application.StartupPath, "Invoices");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(
                    folder,
                    "Invoice_" + _savedOrderId + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf"
                );

                InvoicePdfHelper.CreateInvoicePdf(filePath, _savedOrderId);

                Process.Start(filePath);

                ActivityLogger.Log("Print Invoice", "Orders", _savedOrderId);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            if (!_isPaid || _savedOrderId == 0)
            {
                MessageBox.Show("Please complete payment first.");
                return;
            }

            try
            {
                string folder = Path.Combine(Application.StartupPath, "Invoices");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(
                    folder,
                    "Invoice_Order_" + _savedOrderId + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf"
                );

                InvoicePdfHelper.CreateInvoicePdf(filePath, _savedOrderId);

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

        private void PaymentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isPaid)
            {
                if (MessageBox.Show(
                    "Payment not completed. Close this form?",
                    "Confirm",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                PaymentCompleted = false;
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}