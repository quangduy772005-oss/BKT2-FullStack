namespace PCM.Application.DTOs;

public class NewsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsPinned { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public List<string> ImageUrls { get; set; } = new();
}

public class CreateNewsRequest
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsPinned { get; set; }
    public List<string>? ImageUrls { get; set; }
}

public class UpdateNewsRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsPinned { get; set; }
    public List<string>? ImageUrls { get; set; }
}
