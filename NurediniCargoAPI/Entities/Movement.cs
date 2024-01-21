using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NurediniCargoAPI.Entities
{
    public class Movement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int GoodsId { get; set; }

        [ForeignKey("GoodsId")]
        public Goods? Goods { get; set; }

        [Required]
        public int SourceWarehouseId { get; set; }

        [ForeignKey("SourceWarehouseId")]
        public Warehouse? SourceWarehouse { get; set; }

        [Required]
        public int DestinationWarehouseId { get; set; }

        [ForeignKey("DestinationWarehouseId")]
        public Warehouse? DestinationWarehouse { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime MovementDate { get; set; }

        [Required]
        public required string MovementBy {  get; set; }

        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
    }
}
