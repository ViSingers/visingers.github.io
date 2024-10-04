namespace ViSingers.Server.Models;

public class Voicebank
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public Dictionary<string, string> Description { get; set; } = [];
    public Singer Singer { get; set; }
    public required VoicebankType Type { get; set; }
    public List<VoicebankLanguage> Languages { get; set; } = [];
    public List<string> SampleUrls { get; set; } = [];
}
