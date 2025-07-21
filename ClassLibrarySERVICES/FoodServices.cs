using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClassLibraryDATA.DTO;
using ClassLibraryDATA.Models;
using ClassLibraryREPO;
using Microsoft.Identity.Client;

namespace ClassLibrarySERVICES
{
    public class FoodServices : IFoodServices
    {
        private readonly IFoodRepositories _repo;

        public FoodServices(IFoodRepositories repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(FoodDTO foodDTO)
        {
            var food = new Food
            {
                Name = foodDTO.Name,
                Description = foodDTO.Description,
                Price = foodDTO.Price,
                ImageUrl = foodDTO.ImageUrl,
                CategoryId = foodDTO.CategoryId,
                IsAvailable = true,
                CreatedAt = DateTime.Now,

            };
            await _repo.AddAsync(food);
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var food = await _repo.GetByIdAsync(id);
            if (food == null)
            {
                throw new KeyNotFoundException("food not found");
            }
            else
            {
                bool result = await _repo.DeleteAsync(id);
                return (result, result ? "food deleted successfully" : "Failed to delete food");
            }
        }

        public async Task<IEnumerable<FoodDTO>> GetAllAsync()
        {
            var food = await _repo.GetAllAsync();
            return food.Select(f => new FoodDTO
            {
                FoodId = f.FoodId,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                ImageUrl = f.ImageUrl,
                CategoryId = f.CategoryId,
                IsAvailable = f.IsAvailable,
                CreatedAt = f.CreatedAt,
                Category = f.Category != null ? new CategoryDTO
                {
                    CategoryId = f.Category.CategoryId,
                    CategoryName = f.Category.CategoryName
                } : null
            });
        }

        public async Task<FoodDTO?> GetByIdAsync(int id)
        {
            var food = await _repo.GetByIdAsync(id);
            if (food == null) return null;
            return new FoodDTO
            {
                FoodId = food.FoodId,
                Name = food.Name,
                Description = food.Description,
                Price = food.Price,
                ImageUrl = food.ImageUrl,
                CategoryId = food.CategoryId,
                IsAvailable = food.IsAvailable,
                CreatedAt = food.CreatedAt,
                Category = food.Category != null ? new CategoryDTO
                {
                    CategoryId = food.Category.CategoryId,
                    CategoryName = food.Category.CategoryName
                } : null
            };
        }

        public async Task<IEnumerable<FoodDTO>> GetByKeywordsAsync(List<string> keywords)
        {
            var result = await _repo.GetAllAsync();

            // Nếu không có từ khóa => trả về toàn bộ
            if (keywords == null || !keywords.Any())
                return result.Select(MapToFoodDto);

            // Tách nhỏ thành các từ riêng (VD: "Cơm gà xối mỡ" -> "Cơm", "gà", "xối", "mỡ")
            var splitKeywords = keywords
                .SelectMany(k => k.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(k => k.Trim().ToLower())
                .Distinct()
                .ToList();

            Console.WriteLine("----- [DEBUG] Keywords đã tách:");
            foreach (var k in splitKeywords)
                Console.WriteLine($"👉 {k}");

            // Tìm món ăn có tên chứa ít nhất 1 từ khóa
            var filteredFoods = result
                .Select(f => new
                {
                    Food = f,
                    Score = splitKeywords.Count(k =>
                        f.Name != null && f.Name.ToLower().Contains(k))
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Food.Name)
                .Select(x => MapToFoodDto(x.Food))
                .ToList();

            Console.WriteLine("----- [DEBUG] Món ăn tìm thấy:");
            foreach (var f in filteredFoods)
                Console.WriteLine($"✅ {f.Name}");

            return filteredFoods;
        }



        private FoodDTO MapToFoodDto(Food f)
        {
            return new FoodDTO
            {
                FoodId = f.FoodId,
                Name = f.Name,
                Price = f.Price,
                Description = f.Description,
                ImageUrl = f.ImageUrl,
                IsAvailable = f.IsAvailable,
                CreatedAt = f.CreatedAt,
                CategoryId = f.CategoryId,
            };
        }

        public async Task UpdateAsync(int id, FoodDTO foodDTO)
        {
            var food = await _repo.GetByIdAsync(id);
            if (food == null)
            {
                throw new KeyNotFoundException("Account not found");
            }

            food.Name = foodDTO.Name;
            food.Description = foodDTO.Description;
            food.Price = foodDTO.Price;
            food.ImageUrl = foodDTO.ImageUrl;
            food.CategoryId = foodDTO.CategoryId;
            food.IsAvailable = foodDTO.IsAvailable;
            food.CreatedAt = DateTime.Now;

            await _repo.UpdateAsync(food);
        }
    }
}
