using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement
{
    public static class CurrentUser
    {
        public static string CNIE { get; set; }
        public static string Username { get; set; }
        public static string Role { get; set; }
        public static byte[] Image { get; set; }
    }
}