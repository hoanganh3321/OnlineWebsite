using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrarySERVICES
{
    public interface IAIRecommendationService
    {
        Task<List<string>> ExtractFoodKeywordsAsync(string userInput);
    }
}
