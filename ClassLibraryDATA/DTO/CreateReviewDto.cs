using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryDATA.DTO
{
    public class CreateReviewDto
    {
        public int ProductId { get; set; }    // FE truyền productId
        public int CustomerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int? ParentId { get; set; }
        public bool IsAdminReply { get; set; }
    }
}
