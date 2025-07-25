using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryDATA.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ClassLibraryREPO
{
    public class ProductReviewRepositories : IProductReviewRepositories
    {
        private readonly FoodDeliverContext _context;
        public ProductReviewRepositories(FoodDeliverContext context)
        {
            _context = context;
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<IEnumerable<ProductReviewViewModel>> GetReviewsAsync(int foodId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.FoodId == foodId && r.ParentId == null)
                .Include(r => r.User)
                .Include(r => r.Replies)
                .ThenInclude(reply => reply.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new ProductReviewViewModel
            {
                Id = r.ReviewId,
                ProductId = r.FoodId,
                CustomerId = r.UserId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt ?? DateTime.Now,
                IsAdminReply = r.IsAdminReply ?? false,
                ParentId = r.ParentId,
                CustomerName = r.User?.FullName ?? "Khách hàng",
                Replies = r.Replies.Select(reply => new ProductReviewViewModel
                {
                    Id = reply.ReviewId,
                    ProductId = reply.FoodId,
                    CustomerId = reply.UserId,
                    Rating = reply.Rating,
                    Comment = reply.Comment,
                    CreatedAt = reply.CreatedAt ?? DateTime.Now,
                    IsAdminReply = reply.IsAdminReply ?? false,
                    ParentId = reply.ParentId,
                    CustomerName = reply.User?.FullName ?? "Admin"
                }).ToList()
            }).ToList();
        }

    }
}
