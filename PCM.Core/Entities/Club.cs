using System;
using System.Collections.Generic;

namespace PCM.Core.Entities;

public class Club : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
