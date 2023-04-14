using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Bussiness.Dto
{
    public class PromotionInput
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100), Unicode(false)]
        public string? Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Discount { get; set; }
    }
}
