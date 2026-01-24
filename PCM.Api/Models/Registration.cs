namespace PCM.Api.Models;

public class Registration
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public Member? Member { get; set; }

    public int ActivityId { get; set; }
    public Activity? Activity { get; set; }

    public DateTime RegisteredAt { get; set; }
}
