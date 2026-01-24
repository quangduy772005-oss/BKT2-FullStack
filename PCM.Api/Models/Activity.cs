namespace PCM.Api.Models;

public class Activity
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Location { get; set; } = null!;

    public int ClubId { get; set; }
    public Club? Club { get; set; }

    public ICollection<Registration>? Registrations { get; set; }
}
