using System;

namespace foodoProject.ViewModels
{
    public class CartViewModel
    {
        public int CartProductId { get; set; } // Sepet Ürünü ID
        public string PName { get; set; }  // Ürün adı
        public decimal Price { get; set; } // Ürün fiyatı
        public int Quantity { get; set; }  // Miktar
        public string RestaurantName { get; set; }
        public decimal Total => Price * Quantity; // Toplam tutar
    }
}
