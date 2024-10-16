namespace ViSingers.Server.Models;

public record SingerDetails
{
    public required string Description { get; set; }
    public List<string> GeneralInfo { get; set; } = [];
    public List<string> TermsOfUse { get; set; } = [];
}
