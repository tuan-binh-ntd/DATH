using Entities;

namespace Bussiness.Dto
{
    public class InstallmentForViewDto : EntityDto
    {
        public decimal Balance { get; set; }
        public int Term { get; set; }
        public decimal Interest { get; set; }
    }
}
