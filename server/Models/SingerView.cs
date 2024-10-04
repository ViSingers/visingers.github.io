namespace ViSingers.Server.Models;
public class SingerView
{
    public required string RepositoryName { get; set; }
    public required string Name { get; set; }
    public required string AvatarUrl { get; set; }
    public int Stars { get; set; }
    public required string CreatorLogin { get; set; }
    public required string CreatorName { get; set; }
    public string? SiteUrl { get; set; }
    public List<string> VoicebankTypes { get; set; } = [];
    public List<string> VoicebankLanguages { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public List<VoicebankView> Voicebanks { get; set; } = [];
    public Dictionary<string, SingerDetails> Details { get; set; } = [];
    public List<string> ImageUrls { get; set; } = [];
    public List<string> VideoUrls { get; set; } = [];
}
