using System.ComponentModel.DataAnnotations;

namespace NurediniCargoAPI.Entities
{
    public class Supplier: EntityBase
    {
        [Required]
        public required string Name { get; set; }

        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        [Required]
        public required string Address { get; set; }
        [Required]
        public required string City { get; set; }
        [Required]
        public required string PostalCode { get; set; }
        public string? Country { get; set; }

        public ICollection<GoodsSupplier> SupplierGoods { get; set; } = new List<GoodsSupplier>();
    }
}
