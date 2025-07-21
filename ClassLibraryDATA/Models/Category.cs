using System;
using System.Collections.Generic;

namespace ClassLibraryDATA.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
}
