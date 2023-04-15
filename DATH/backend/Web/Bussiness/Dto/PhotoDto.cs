using Entities;
using System.ComponentModel.DataAnnotations;

namespace Bussiness.Dto
{
    public class PhotoDto : EntityDto
    {
        [Required, StringLength(500), DataType(DataType.Url)]
        public string? Url { get; set; }
    }
}
