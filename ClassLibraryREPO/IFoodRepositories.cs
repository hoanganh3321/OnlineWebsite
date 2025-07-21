using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;

namespace ClassLibraryREPO
{
    public interface IFoodRepositories
    {
       
        Task<IEnumerable<Food>> GetAllAsync();
        Task<Food?> GetByIdAsync(int foodid);
        Task<bool> AddAsync(Food food);
        Task<bool> UpdateAsync(Food food);
        Task<bool> DeleteAsync(int id);
    }
}
