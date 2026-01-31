using PCM.Core.Enums;

namespace PCM.Core.Entities;

public class Court : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal PricePerHour { get; set; }
    public int Capacity { get; set; } // Number of players

    // Navigation
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public class Booking : BaseEntity
{
    public int CourtId { get; set; }
    public int MemberId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.PendingPayment;
    public string? Note { get; set; }
    public byte[]? RowVersion { get; set; } // For optimistic locking (concurrency control)

    // Navigation
    public virtual Court Court { get; set; } = null!;
    public virtual Member Member { get; set; } = null!;
}

/// <summary>
/// Recurring booking template for regular players
/// </summary>
public class RecurringBooking : BaseEntity
{
    public int MemberId { get; set; }
    public int CourtId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal PricePerSession { get; set; }
    public string DaysOfWeek { get; set; } = null!; // e.g., "1,3,5" for Mon, Wed, Fri
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigation
    public virtual Member Member { get; set; } = null!;
    public virtual Court Court { get; set; } = null!;
}
