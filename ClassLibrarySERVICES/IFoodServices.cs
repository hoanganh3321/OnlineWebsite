using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.DTO;

namespace ClassLibrarySERVICES
{
    public interface IFoodServices
    {
        Task<IEnumerable<FoodDTO>> GetAllAsync();
        Task<FoodDTO?> GetByIdAsync(int id);
        Task CreateAsync(FoodDTO foodDTO);
        Task UpdateAsync(int id, FoodDTO foodDTO);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<IEnumerable<FoodDTO>> GetByKeywordsAsync(List<string> keywords);
    }
}
