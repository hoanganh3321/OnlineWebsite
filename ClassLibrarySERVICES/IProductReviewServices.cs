using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryDATA.ViewModels;

namespace ClassLibrarySERVICES
{
    public interface IProductReviewServices
    {
        Task<IEnumerable<ProductReviewViewModel>> GetReviewsAsync(int productId);
        Task<Review> CreateReviewAsync(Review review);
    }
}
