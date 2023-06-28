using Bussiness.Services.Core;

namespace Bussiness.Dto
{
    public class GetOrderForShopInput : PaginationInput
    {
        public int ShopId { get; set; }
    }
}
