namespace PCM.Api.Models;

public class Club
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Member>? Members { get; set; }
    public ICollection<Activity>? Activities { get; set; }
}
