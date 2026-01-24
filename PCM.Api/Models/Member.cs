namespace PCM.Api.Models;

public class Member
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime JoinDate { get; set; }

    public int ClubId { get; set; }
    public Club? Club { get; set; }

    public ICollection<Registration>? Registrations { get; set; }
}
