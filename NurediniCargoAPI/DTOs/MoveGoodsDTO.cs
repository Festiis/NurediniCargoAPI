namespace NurediniCargoAPI.DTOs
{
    public class MoveGoodsDTO
    {
        public required int GoodsId { get; set; }
        public required int SourceWarehouseId { get; set; }
        public required int DestinationWarehouseId { get; set; }
        public required int Quantity { get; set; }

        public required string CreatedBy { get; set; }
        public required DateTime CreatedDate { get; set; }
    }
}
