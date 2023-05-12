using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class WarehouseDetail : BaseEntity<long>
    {
        public long ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public DateTime ActualDate { get; set; }
        public Warehouse? Warehouse { get; set; }
        public Product? Product { get; set; }
    }
}
