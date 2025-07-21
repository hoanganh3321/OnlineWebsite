using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibraryDATA.Models;

namespace ClassLibraryREPO
{
    public interface ICategoryRepositories
    {
        Task<IEnumerable<Category>> GetAllCategoryAsync();
    }
}
