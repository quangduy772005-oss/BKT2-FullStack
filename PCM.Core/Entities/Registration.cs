using System;

namespace PCM.Core.Entities;

public class Registration : BaseEntity
{
    public int MemberId { get; set; }
    public virtual Member Member { get; set; } = null!;

    public int ActivityId { get; set; }
    public virtual Activity Activity { get; set; } = null!;

    public DateTime RegisteredAt { get; set; }
}
