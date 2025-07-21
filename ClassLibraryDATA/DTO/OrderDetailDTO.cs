using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryDATA.DTO
{
    public class OrderDetailDTO
    {
        public int Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public FoodDTO Food { get; set; } 
    }
}
