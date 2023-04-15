namespace Bussiness.Dto
{
    public class ProductInput
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int? ProductCategoryId { get; set; }
    }
}
