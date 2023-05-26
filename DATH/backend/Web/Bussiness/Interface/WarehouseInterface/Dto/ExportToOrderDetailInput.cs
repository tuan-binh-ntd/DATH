namespace Bussiness.Interface.WarehouseInterface.Dto
{
    public class ExportToOrderDetailInput
    {
        public int Quantity { get; set; }
        public long ProductId { get; set; }
        public string? Color { get; set; }
    }
}
