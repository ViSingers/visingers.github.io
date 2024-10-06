using Microsoft.EntityFrameworkCore;
using ViSingers.Server;
using ViSingers.Server.Services;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("Running in Development mode");
    var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection") ?? throw new InvalidOperationException("Connection string 'SQLiteConnection' not found.");
    builder.Services.AddDbContext<ApplicationContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    Console.WriteLine("Running in Production mode");
    var connectionString = builder.Configuration.GetConnectionString("MySQLConnection") ?? throw new InvalidOperationException("Connection string 'MySQLConnection' not found.");
    var mysqlVersion = builder.Configuration.GetConnectionString("MySQLVersion") ?? throw new InvalidOperationException("'MySQLVersion' not found.");
    builder.Services.AddDbContext<ApplicationContext>(options =>
        options.UseMySql(connectionString, ServerVersion.Parse(mysqlVersion)));
}

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var frontendUrl = builder.Configuration.GetValue<string>("FrontendUrl");

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins(frontendUrl);
                      });
});
builder.Services.AddLettuceEncrypt();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<GitHubParserService>();
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors(MyAllowSpecificOrigins);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
