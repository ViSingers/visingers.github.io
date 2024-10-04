namespace ViSingers.Server.Models;

public class VoicebankView
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public Dictionary<string, string> Description { get; set; } = [];
    public required string Type { get; set; }
    public List<string> Languages { get; set; } = [];
    public List<string> SampleUrls { get; set; } = [];
}
