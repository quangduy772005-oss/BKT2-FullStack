namespace PCM.Core.Entities;

public class News : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsPinned { get; set; }
    public string? CreatedBy { get; set; }

    // Navigation
    public virtual ICollection<NewsImage> Images { get; set; } = new List<NewsImage>();
}

public class NewsImage : BaseEntity
{
    public int NewsId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int DisplayOrder { get; set; }

    // Navigation
    public virtual News News { get; set; } = null!;
}
