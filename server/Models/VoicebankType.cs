namespace ViSingers.Server.Models;

public class VoicebankType
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Voicebank> Voicebanks { get; set; } = [];
}