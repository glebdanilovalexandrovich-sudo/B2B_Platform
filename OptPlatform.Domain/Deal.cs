
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace OptPlatform.Domain
{
    public class Deal
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        
        public User Buyer { get; set; }
        public User Supplier { get; set; }
        public List<DealItem> Items { get; set; }
    }
}
