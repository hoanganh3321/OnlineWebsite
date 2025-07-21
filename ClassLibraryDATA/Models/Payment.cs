using System;
using System.Collections.Generic;

namespace ClassLibraryDATA.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public decimal AmountPaid { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual Order Order { get; set; } = null!;
}
