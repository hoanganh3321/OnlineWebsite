using ClassLibraryDATA.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibraryREPO
{
    public class FoodRepositories : IFoodRepositories
    {
        private readonly FoodDeliverContext _context;
        public FoodRepositories(FoodDeliverContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Food food)
        {
            await _context.Foods.AddAsync(food);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return false;
            }
            _context.Foods.Remove(food);
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<IEnumerable<Food>> GetAllAsync()
        {
            return await _context.Foods.ToListAsync();
        }

        public async Task<Food?> GetByIdAsync(int foodid)
        {
            return await _context.Foods
            .Include(f => f.Category) // Tải trước Category
            .FirstOrDefaultAsync(f => f.FoodId == foodid);
        }

        public async Task<bool> UpdateAsync(Food food)
        {
            _context.Foods.Update(food);
            return await _context.SaveChangesAsync() > 0;

        }
    }
}
