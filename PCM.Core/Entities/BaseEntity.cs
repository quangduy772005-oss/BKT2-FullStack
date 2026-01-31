namespace PCM.Core.Entities;

/// <summary>
/// Base entity for all domain entities
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;
}
