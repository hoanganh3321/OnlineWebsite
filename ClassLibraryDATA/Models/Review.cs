using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibraryDATA.Models;

public partial class Review
{
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public int FoodId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool? IsAdminReply { get; set; }
    public int? ParentId { get; set; }

    public virtual Food? Food { get; set; }
    public virtual User? User { get; set; }

    [ForeignKey(nameof(ParentId))] 
    public virtual Review? ParentReview { get; set; }

    public virtual ICollection<Review> Replies { get; set; } = new List<Review>();
}
