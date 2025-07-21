using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibraryREPO
{
    public class CategoryRepositories : ICategoryRepositories
    {
        private readonly FoodDeliverContext _context;
        public CategoryRepositories(FoodDeliverContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllCategoryAsync()
        {
            return await _context.Categories.ToListAsync();
        }

    }
}
