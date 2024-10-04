namespace ViSingers.Server.Models;

public class VoicebankLanguage
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string FullName { get; set; }
    public List<Voicebank> Voicebanks { get; set; } = [];
}
