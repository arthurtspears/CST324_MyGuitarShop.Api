using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Data.Ado.Entities
{
    public class AddressEntity
    {
        public required int AddressID { get; set; }

        public required int CustomerID { get; set; }

        [MaxLength(60)]
        public required string Line1 { get; set; }

        [MaxLength(60)]
        public string? Line2 { get; set; } = null;

        [MaxLength(40)]
        public required string City { get; set; }

        [MaxLength(2)]
        public required string State { get; set; }

        [MaxLength(10)]
        public required string ZipCode { get; set; }

        [MaxLength(12)]
        public required string Phone { get; set; }

        public required bool Disabled { get; set; } = false;
    }
}
