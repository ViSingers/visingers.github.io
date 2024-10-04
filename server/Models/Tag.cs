namespace ViSingers.Server.Models;

public class Tag
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Singer> Singers { get; set; } = [];
}