using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? UserId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime? SubmittedDate { get; set; }

    public virtual User? User { get; set; }
}
