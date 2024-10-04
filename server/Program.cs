using Microsoft.EntityFrameworkCore;
using ViSingers.Server;
using ViSingers.Server.Services;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection") ?? throw new InvalidOperationException("Connection string 'SQLiteConnection' not found.");
    builder.Services.AddDbContext<ApplicationContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("MySQLConnection") ?? throw new InvalidOperationException("Connection string 'MySQLConnection' not found.");
    var mysqlVersion = builder.Configuration.GetConnectionString("MySQLVersion") ?? throw new InvalidOperationException("'MySQLVersion' not found.");

    builder.Services.AddDbContext<ApplicationContext>(options =>
        options.UseMySql(connectionString, ServerVersion.Parse(mysqlVersion)));
}
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<GitHubParserService>();
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
