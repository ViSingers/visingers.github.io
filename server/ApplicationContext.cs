using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;
using System.Text.Json;
using System.Xml;
using ViSingers.Server.Extensions;
using ViSingers.Server.Models;

namespace ViSingers.Server;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Singer> Singers => Set<Singer>();
    public DbSet<Voicebank> Voicebank => Set<Voicebank>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<VoicebankLanguage> VoicebankLanguages => Set<VoicebankLanguage>();
    public DbSet<VoicebankType> VoicebankTypes => Set<VoicebankType>();

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
        /*var tags = new List<Tag> { new Tag { Name = "kawaii" }, new Tag { Name = "help" }, new Tag { Name = "me" } };
        var langs = new List<VoicebankLanguage> { new VoicebankLanguage { Name = "jp", FullName = "japanese" }, new VoicebankLanguage { Name = "ru", FullName = "russian" } };
        var types = new List<VoicebankType> { new VoicebankType { Name = "utau" }, new VoicebankType { Name = "diffsinger" }, new VoicebankType { Name = "rvc" } };
        var rand = new Random();
        for (var i = 1; i <= 1000; i++)
        {
            var user = new User { Login = "testuser" + i, Name = "Test User " + i };
            var singer = new Singer { VideoUrls = ["k4T8HeK-ZIg", "k4T8HeK-ZIg", "k4T8HeK-ZIg", "k4T8HeK-ZIg", "k4T8HeK-ZIg"], ImageUrls = ["https://64.media.tumblr.com/6bd9cceaa93c34aa5edf16fe0a918e1f/9f650d017973cf63-d8/s2048x3072/845521db099f325cf2dfa808a88f80785aae0b40.jpg", "https://i.pinimg.com/originals/7e/d4/43/7ed44319117e6ed20df0f8dc3c4db7cd.jpg", "https://i.pinimg.com/originals/cf/10/34/cf10345fff1c77ad5a98b9dc692369b0.jpg"], Voicebanks = [new Voicebank { Name = "Testbank 1", Type = types[rand.Next(3)], Description = { { "en", "Test description" }, { "ru", "Тут сто аппендов верьте" } }, SampleUrls = ["https://zvukogram.com/mp3/cats/2738/avtomobil-buduschego-rezkiy-blizkiy.mp3", "https://zvukogram.com/mp3/cats/2738/avtomobil-buduschego-rezkiy-nudnyiy.mp3"], Languages = langs, Url = "https://github.com/Megageorgio/hhskt_ru_phonemizer/releases/download/v1.0.0/diffs_ru_hhskt.dll" }, new Voicebank { Name = "Testbank 2", Type = types[rand.Next(3)], Description = { { "en", "Test description again" }, { "ru", "Тут только один аппенд т_т" } }, SampleUrls = ["https://zvukogram.com/mp3/cats/2738/avtomobil-buduschego-rezkiy-blizkiy.mp3", "https://zvukogram.com/mp3/cats/2738/avtomobil-buduschego-rezkiy-nudnyiy.mp3"], Languages = langs, Url = "https://github.com/Megageorgio/hhskt_ru_phonemizer/releases/download/v1.0.0/diffs_ru_hhskt.dll" }], Stars = rand.Next(50), Tags = tags, AvatarUrl = "https://raw.githubusercontent.com/ViSingers/testsinger/refs/heads/main/avatar.png", Creator = user, Name = "Test Test " + i, Details = new Dictionary<string, SingerDetails>() { { "en", new SingerDetails { Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", GeneralInfo = ["test1: aaaaa", "test2: bbbbb", "test3: ccccc"] } }, { "ru", new SingerDetails { Description = "Помогите я не спал два дня", GeneralInfo = ["один: два", "три: четыре", "пять: двадцать один"] } } }, RepositoryName = "testsinger" + i, UpdatedAt = DateTime.UtcNow };
            Users.Add(user);
            Singers.Add(singer);
        }*/
    }

    public static IEnumerable<(string Code, string Name)> GetLanguages()
    {
        var languages = new HashSet<(string, string)>();

        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

        foreach (var culture in cultures)
        {
            if (!culture.IsNeutralCulture && culture.TwoLetterISOLanguageName.Length == 2)
            {
                string languageName = culture.EnglishName.ToLower().Split().First();
                string isoCode = culture.TwoLetterISOLanguageName;
                languages.Add((isoCode, languageName));
            }
        }

        return languages.OrderBy(l => l);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Singer>().ConfigurePropertyAsJson(e => e.Details);
        modelBuilder.Entity<Group>().ConfigurePropertyAsJson(e => e.Description);
        modelBuilder.Entity<Voicebank>().ConfigurePropertyAsJson(e => e.Description);

        var languages = GetLanguages().Select((lang, i) => new VoicebankLanguage { Id = i + 1, Name = lang.Code, FullName = lang.Name });
        var types = new string[] { "utau", "paintvoice", "diffsinger", "rvc", "freeloid", "coeiroink", "vocalsharp", "niaoniao", "deepvocal" }
        .Select((type, i) => new VoicebankType { Id = i + 1, Name = type });

        modelBuilder.Entity<VoicebankLanguage>().HasData(languages);
        modelBuilder.Entity<VoicebankType>().HasData(types);

        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                            || p.PropertyType == typeof(DateTimeOffset?));
                foreach (var property in properties)
                {
                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
                }
            }
        }
    }
}
