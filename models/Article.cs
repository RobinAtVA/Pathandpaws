namespace VisionaryAnalytics.Models;

public class Article
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string Slug { get; set; } = "";

    public string Summary { get; set; } = "";

    public string Content { get; set; } = "";

    public bool Published { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}