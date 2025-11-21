using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.DTOs
{
    public class AddressDto
    {
        public int? AddressID { get; set; } = null;

        public int? CustomerID { get; set; } = null;

        [MaxLength(60)]
        public string? Line1 { get; set; }

        [MaxLength(60)]
        public string? Line2 { get; set; } = null;

        [MaxLength(40)]
        public string? City { get; set; }

        [MaxLength(2)]
        public string? State { get; set; }

        [MaxLength(10)]
        public string? ZipCode { get; set; }

        [MaxLength(12)]
        public string? Phone { get; set; }

        public bool Disabled { get; set; } = false;
    }
}
