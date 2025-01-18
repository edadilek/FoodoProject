using System;

namespace foodoProject.ViewModels
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public DateTime ODate { get; set; }
        public string OStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public string RestaurantName { get; set; } // Yeni özellik
    }
}

