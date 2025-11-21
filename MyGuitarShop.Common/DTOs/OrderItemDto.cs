namespace MyGuitarShop.Common.DTOs
{
    public class OrderItemDto
    {
        public int? ItemID { get; set; } = null;
        public int? OrderID { get; set; } = null;
        public int? ProductID { get; set; } = null;
        public decimal ItemPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public int Quantity { get; set; }
    }
}