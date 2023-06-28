using Bussiness.Dto;
using Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bussiness.Interface.OrderInterface.Dto
{
    public class OrderDetailForViewDto : EntityDto<long>
    {
        [Required]
        public decimal Cost { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string? SpecificationId { get; set; }
        public long ProductId { get; set; }
        public int? InstallmentId { get; set; }
        [StringLength(500)]
        public string? ProductName { get; set; }
        [Column(TypeName = "decimal(19, 5)")]
        public decimal Price { get; set; }
        public ICollection<PhotoDto>? Photos { get; set; }
        public ICollection<SpecificationDto>? Specifications { get; set; }
    }
}
