using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.DTOs
{
    public class AdministratorDto
    {
        public int? AdminID { get; set; } = null;

        [MaxLength(255)]
        public string? EmailAddress { get; set; }

        [MaxLength(255)]
        public string? Password { get; set; }

        [MaxLength(255)]
        public string? FirstName { get; set; }

        [MaxLength(255)]
        public string? LastName { get; set; }
    }
}