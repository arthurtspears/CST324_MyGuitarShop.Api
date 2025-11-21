using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.DTOs
{
    public class CustomerDto
    {
        public int? CustomerID { get; set; } = null;
        
        [MaxLength(255)]
        public string? EmailAddress { get; set; }
    
        [MaxLength(60)]
        public string? Password { get; set; }
 
        [MaxLength(60)]
        public string? FirstName { get; set; }

        [MaxLength(60)]
        public string? LastName { get; set; }

        public int? ShipAddressID { get; set; } = null;

        public int? BillingAddressID { get; set; } = null;
    }
}