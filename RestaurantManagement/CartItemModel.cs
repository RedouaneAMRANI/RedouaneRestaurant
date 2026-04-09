using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement
{
    public class CartItemModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public float UnitPrice { get; set; }
        public int Quantity { get; set; }
        public byte[] Image { get; set; }

        public float Total
        {
            get { return UnitPrice * Quantity; }
        }
    }
}