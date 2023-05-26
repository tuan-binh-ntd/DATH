namespace Bussiness.Interface.WarehouseInterface.Dto
{
    public class ExportToOrderInput
    {
        public string? OrderCode { get; set; }
        public ICollection<ExportToOrderDetailInput>? ExportToOrderDetailInputs { get; set; }

    }
}
