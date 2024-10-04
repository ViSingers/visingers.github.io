namespace ViSingers.Server.Models;

public class User
{
    public int Id { get; set; }
    public required string Login { get; set; }
    public string? Name { get; set; }
    public List<Singer> Singers { get; set; } = [];
}