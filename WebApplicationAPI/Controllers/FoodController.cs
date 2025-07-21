using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;
using ClassLibrarySERVICES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodServices _foodServices;
        private readonly ICategoryServices _categoryServices;

        public FoodController(IFoodServices foodServices, ICategoryServices categoryServices)
        {
            _foodServices = foodServices;
            _categoryServices = categoryServices;
        }
        //https://localhost:7224/api/food/list
        [HttpGet("list")]
        public async Task<IActionResult> GetAllFood()
        {
            var food = await _foodServices.GetAllAsync();
            return Ok(food);
        }       
        //https://localhost:7224/api/food/search-by-keywords
        [HttpGet("search-by-keywords")]
        public async Task<IActionResult> GetByKeywords([FromQuery] List<string> keyword)
        {
            var foods = await _foodServices.GetByKeywordsAsync(keyword);
            return Ok(foods);
        }

        //https://localhost:7224/api/food/detail/{foodId}
        [HttpGet("detail/{foodId}")]
        public async Task<ActionResult<FoodDTO>> GetFoodById(int foodId)
        {
            var cate = await _categoryServices.GetAllCategoriesAsync();
            if (foodId<=0) return BadRequest();
            var food = await _foodServices.GetByIdAsync(foodId);
            if (food == null) return NotFound();
            return Ok(food);
        }
    }
}
