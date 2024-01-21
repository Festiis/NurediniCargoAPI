namespace NurediniCargoAPI.DTOs
{
    public class InventoryDTO
    {
        public required int WarehouseId { get; set; }
        public required int GoodsId { get; set; }
        public required int Quantity { get; set; }

        public required string CreatedBy { get; set; }
        public required DateTime CreatedDate { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
