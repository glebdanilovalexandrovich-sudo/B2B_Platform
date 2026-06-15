using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace OptPlatform.Domain
{
    public class DealItem
    {
        public int Id { get; set; }
        public int DealId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtMoment { get; set; }

        public Deal Deal { get; set; }

        public Product Product { get; set; }
    }
}
