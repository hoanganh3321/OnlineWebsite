using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;
using ClassLibraryREPO;

namespace ClassLibrarySERVICES
{
    public class CategoryServices : ICategoryServices
    {
        private readonly ICategoryRepositories _categoryRepo;
        public CategoryServices(ICategoryRepositories categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepo.GetAllCategoryAsync();
        }
    }
}
