using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryDATA.ViewModels;

namespace ClassLibraryREPO
{
    public interface IProductReviewRepositories
    {
        Task<IEnumerable<ProductReviewViewModel>> GetReviewsAsync(int foodId);
        Task<Review> AddReviewAsync(Review review);
    }
}
