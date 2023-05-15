using Microsoft.AspNetCore.Http;

namespace Bussiness.Dto
{
    public class PhotoInput
    {
        public bool IsMain { get; set; }
        public IFormFile? File  { get; set; }
    }
}
