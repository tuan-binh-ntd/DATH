using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dto
{
    public class ProductForViewDto : EntityDto<long>
    {
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
