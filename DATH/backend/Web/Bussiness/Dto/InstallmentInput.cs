using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Dto
{
    public class InstallmentInput
    {
        public decimal Balance { get; set; }
        public int Term { get; set; }
    }
}
