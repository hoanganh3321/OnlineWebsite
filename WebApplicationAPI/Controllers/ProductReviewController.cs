using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;
using ClassLibrarySERVICES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace WebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IProductReviewServices _service;
        private readonly IHubContext<ReviewHub> _hubContext;

        public ProductReviewController(IProductReviewServices service, IHubContext<ReviewHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet("{foodId}")]
        public async Task<IActionResult> GetReviews(int foodId)
        {
            var reviews = await _service.GetReviewsAsync(foodId);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ Map thủ công, chỉ gán FoodId & UserId
            var review = new Review
            {
                FoodId = dto.ProductId,
                UserId = dto.CustomerId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ParentId = dto.ParentId,
                IsAdminReply = dto.IsAdminReply,
                CreatedAt = DateTime.Now
            };

            var newReview = await _service.CreateReviewAsync(review);

            await _hubContext.Clients.Group($"product_{review.FoodId}")
                .SendAsync("ReceiveReview", newReview);

            return Ok(newReview);
        }

    }
}
