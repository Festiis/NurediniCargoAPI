using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NurediniCargoAPI.Entities
{
    public class GoodsSupplier
    {
        [Required]
        public int GoodsId { get; set; }

        [ForeignKey("GoodsId")]
        public Goods? Goods { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
    }
}