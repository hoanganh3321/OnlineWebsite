using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ClassLibraryDATA.Models;

namespace ClassLibraryDATA.DTO
{
    public class FoodDTO
    {
        public int FoodId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public CategoryDTO? Category { get; set; } // Sử dụng CategoryDTO thay vì Category
    }
}
