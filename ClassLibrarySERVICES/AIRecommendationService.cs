using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassLibrarySERVICES
{
    public class AIRecommendationService : IAIRecommendationService
    {
        private readonly HttpClient _httpClient;
        public AIRecommendationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<string>> ExtractFoodKeywordsAsync(string userInput)
        {
            var requestBody = new { userInput };
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/suggest", requestBody);

            if (!response.IsSuccessStatusCode)
                return new List<string>();

            var rawSuggestions = await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
            var keywords = new List<string>();

            foreach (var line in rawSuggestions)
            {
                // Lấy tên món trước dấu phân cách
                string clean = line.Trim();
                string[] separators = { "–", "-", ":" };
                foreach (var sep in separators)
                {
                    if (clean.Contains(sep))
                    {
                        clean = clean.Split(sep)[0].Trim();
                        break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(clean))
                    keywords.Add(clean);
            }

            // ✅ Thêm cả keyword chính (cơm, bún, phở...) để tăng khả năng match DB
            var mainKeywords = keywords
                .SelectMany(k => k.Split(' '))
                .Where(word => word.Length > 2 && char.IsLetter(word[0]))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            keywords.AddRange(mainKeywords);

            return keywords.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }


    }
}
