using ClassLibraryDATA.DTO;
using ClassLibrarySERVICES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIRecommendationService _aiService;
        public AIController(IAIRecommendationService aiService)
        {
            _aiService = aiService;
        }
        //  https://localhost:7224/api/AI/suggest
        [HttpPost("suggest")]
        public async Task<IActionResult> Suggest([FromBody] AIRequestDto dto)
        {
            var keywords = await _aiService.ExtractFoodKeywordsAsync(dto.UserInput);
            return Ok(keywords);
        }
    }
}
