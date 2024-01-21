using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NurediniCargoAPI.Entities
{
    public class Inventory : EntityBase
    {
        [Required]
        public int WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public Warehouse? Warehouse { get; set; }

        [Required]
        public int GoodsId { get; set; }

        [ForeignKey("GoodsId")]
        public Goods? Goods { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
