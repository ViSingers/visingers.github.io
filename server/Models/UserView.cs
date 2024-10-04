namespace ViSingers.Server.Models;

public class UserView
{
    public required string Login { get; set; }
    public string? Name { get; set; }
    public List<SingerView> Singers { get; set; } = [];
}
