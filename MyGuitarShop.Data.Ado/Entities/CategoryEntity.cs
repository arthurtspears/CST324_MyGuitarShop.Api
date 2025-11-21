using System.ComponentModel.DataAnnotations;
using MyGuitarShop.Common.Enums;

namespace MyGuitarShop.Data.Ado.Entities
{
    public class CategoryEntity
    {
        public required CategoryType CategoryID { get; set; }

        [MaxLength(255)]
        public required string CategoryName { get; set; }
    }
}