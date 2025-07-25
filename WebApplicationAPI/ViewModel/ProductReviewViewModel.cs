using System.ComponentModel.DataAnnotations.Schema;
using ClassLibraryDATA.Models;

namespace WebApplicationAPI.ViewModel
{
    public class ProductReviewViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int? ParentId { get; set; }
        public bool IsAdminReply { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CustomerName { get; set; }

        public List<ProductReviewViewModel> Replies { get; set; } = new();
    }

}
