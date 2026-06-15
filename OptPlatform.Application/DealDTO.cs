using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OptPlatform.Application
{
    public class DealDTO
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DealItemDTO> Items { get; set; }
    }
}
