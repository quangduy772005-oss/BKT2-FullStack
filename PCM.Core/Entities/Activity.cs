using System;
using System.Collections.Generic;

namespace PCM.Core.Entities;

public class Activity : BaseEntity
{
    public string Title { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Location { get; set; } = null!;

    public int ClubId { get; set; }
    public virtual Club Club { get; set; } = null!;

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
