using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.DTOs
{
    public class OrderDto
    {
        public int? OrderID { get; set; } = null;

        public int CustomerID { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal ShipAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public DateTime? ShipDate { get; set; } = null;

        public int ShipAddressID { get; set; }

        [MaxLength(50)]
        public string? CardType { get; set; }

        [MaxLength(16)]
        public string? CardNumber { get; set; }

        [MaxLength(7)]
        public string? CardExpires { get; set; }

        public int BillingAddressID { get; set; }

        public IEnumerable<OrderItemDto> OrderItems { get; set; } = [];
    }
}
