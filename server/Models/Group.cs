namespace ViSingers.Server.Models;

public class Group
{
    public int Id { get; set; }
    public required string RepositoryName { get; set; }
    public required string Name { get; set; }
    public Dictionary<string, string> Description { get; set; } = [];
    public required string AvatarUrl { get; set; }
    public string? SiteUrl { get; set; }
    public required DateTime UpdatedOn { get; set; }
    public required User Creator { get; set; }
    public List<Singer> Singers { get; set; } = [];
}
