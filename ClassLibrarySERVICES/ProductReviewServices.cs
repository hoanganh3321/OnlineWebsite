using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryDATA.ViewModels;
using ClassLibraryREPO;

namespace ClassLibrarySERVICES
{
    public class ProductReviewServices : IProductReviewServices
    {
        private readonly IProductReviewRepositories _repo;

        public ProductReviewServices(IProductReviewRepositories repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ProductReviewViewModel>> GetReviewsAsync(int productId)
        => await _repo.GetReviewsAsync(productId);

        public async Task<Review> CreateReviewAsync(Review review)
        {
            review.CreatedAt = DateTime.UtcNow;
            return await _repo.AddReviewAsync(review);
        }
    }
}
