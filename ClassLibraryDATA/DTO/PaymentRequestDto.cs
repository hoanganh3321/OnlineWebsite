using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryDATA.DTO
{
    public class PaymentRequestDto
    {
        [Required]
        public int OrderId { get; set; } 

        [Required]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 1000 VND")]
        public decimal Amount { get; set; } 
    }
}
