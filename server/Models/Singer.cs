namespace ViSingers.Server.Models;

public class Singer
{
    public int Id { get; set; }
    public required string RepositoryName { get; set; }
    public required string Name { get; set; }
    public Dictionary<string, SingerDetails> Details { get; set; } = [];
    public required string AvatarUrl { get; set; }
    public string? SiteUrl { get; set; }
    public int Stars { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required User Creator { get; set; }
    public List<Group> Groups { get; set; } = [];
    public List<Voicebank> Voicebanks { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    public List<string> ImageUrls { get; set; } = [];
    public List<string> VideoUrls { get; set; } = [];
}
