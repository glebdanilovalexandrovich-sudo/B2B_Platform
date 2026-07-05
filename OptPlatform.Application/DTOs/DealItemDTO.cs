using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptPlatform.Application
{
    public class DealItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtMoment { get; set; }
        public decimal Total => PriceAtMoment * Quantity;
    }
}
