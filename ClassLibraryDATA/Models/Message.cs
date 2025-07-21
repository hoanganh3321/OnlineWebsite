using System;
using System.Collections.Generic;

namespace ClassLibraryDATA.Models;

public partial class Message
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public bool IsReadd { get; set; }
}
