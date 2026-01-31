namespace PCM.Application.DTOs;

#region Court & Booking DTOs

public class CourtDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal PricePerHour { get; set; }
    public int Capacity { get; set; }
}

public class BookingSlotDto
{
    public int CourtId { get; set; }
    public string CourtName { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}

public class BookingDto
{
    public int Id { get; set; }
    public int CourtId { get; set; }
    public string CourtName { get; set; } = null!;
    public int MemberId { get; set; }
    public string MemberName { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = null!;
    public string? Note { get; set; }
}

public class CreateBookingRequest
{
    public int CourtId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Note { get; set; }
}

public class CreateRecurringBookingRequest
{
    public int CourtId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal PricePerSession { get; set; }
    public string DaysOfWeek { get; set; } = null!; // "1,3,5" for Mon, Wed, Fri
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class RecurringBookingSlotResult
{
    public List<string> SuccessDates { get; set; } = new();
    public List<ConflictSlot> ConflictDates { get; set; } = new();
}

public class ConflictSlot
{
    public DateTime Date { get; set; }
    public string? ConflictWithMember { get; set; }
}

public class CancelBookingRequest
{
    public int BookingId { get; set; }
}

#endregion
