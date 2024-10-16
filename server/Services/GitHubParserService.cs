using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.EntityFrameworkCore;
using Octokit;
using System.Text.Json;
using System.Text.RegularExpressions;
using mk.profanity;
using ViSingers.Server.Models;
using User = ViSingers.Server.Models.User;

namespace ViSingers.Server.Services;

public partial class GitHubParserService : IHostedService, IDisposable
{
    private readonly ILogger<GitHubParserService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly GitHubClient _gitHubClient;
    private readonly GraphQLHttpClient _graphqlClient;
    private readonly ApplicationContext _context;
    private readonly ProfanityFilter _profanityFilter;

    private readonly static Regex YoutubeRegex = new Regex(@"(?:https?:\/\/)?(?:www\.)?(?:(?:youtube\.com\/watch\?v=)|(?:youtu\.be\/))([a-zA-Z0-9_-]{11})", RegexOptions.Compiled);
    private readonly static string AppName = "ViSingersBot";
    private readonly static string GithubRawUrl = "https://raw.githubusercontent.com/";
    private readonly static string GetRepoQuery = """
    query($owner: String!, $name: String!) {
      repository(owner: $owner, name: $name) {
        object(expression: "HEAD:") {
          ... on Tree {
            entries {
              name
              type
              path
              object {
                ... on Blob {
                  text
                  byteSize
                }
              }
            }
          }
        }
      }
    }

    """;

    public GitHubParserService(ILogger<GitHubParserService> logger, IServiceProvider serviceProvider, IServiceScopeFactory factory)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _gitHubClient = new GitHubClient(new ProductHeaderValue(AppName));
        _graphqlClient = new GraphQLHttpClient("https://api.github.com/graphql", new SystemTextJsonSerializer());
        var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        var tokenAuth = new Credentials(githubToken);
        _graphqlClient.HttpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", githubToken);
        _graphqlClient.HttpClient.DefaultRequestHeaders.UserAgent.Add(
            new System.Net.Http.Headers.ProductInfoHeaderValue(AppName, "1.0"));
        _gitHubClient.Credentials = tokenAuth;
        _profanityFilter = new ProfanityFilter();
        _context = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GitHub Repo Parser Service starting.");
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        Task.Run(async () =>
        {
            while (await timer.WaitForNextTickAsync())
            {
                await ParseGitHubRepos();
            }
        });
        return Task.CompletedTask;
    }

    static async Task<string[]> ReadFileFromUrlAsync(string url)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                string content = await httpClient.GetStringAsync(url);

                string[] lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                return lines;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Can't read file: {e.Message}");
                return Array.Empty<string>();
            }
        }
    }

    record SectionInfo(string Name, string[] Content);

    static List<SectionInfo> GetSections(string[] rows)
    {
        var sections = new List<SectionInfo>();
        var sectionName = string.Empty;
        var content = new List<string>();
        foreach (var row in rows)
        {
            if (row.StartsWith("#"))
            {
                if (!string.IsNullOrEmpty(sectionName))
                {
                    sections.Add(new SectionInfo(sectionName, content.ToArray()));
                }
                sectionName = row.TrimStart('#').Split("[!").First().Trim();
                content = [];
            }
            else if (!string.IsNullOrEmpty(sectionName))
            {
                content.Add(row);
            }
        }

        if (content.Count != 0)
        {
            sections.Add(new SectionInfo(sectionName, content.ToArray()));
        }

        return sections;
    }

    private async Task ParseGitHubRepos()
    {
        var videosSectionName = "videos";
        var groupsSectionName = "groups";
        var termsOfUseSectionName = "terms of use";
        var sectionNames = new List<string> { videosSectionName, groupsSectionName, termsOfUseSectionName };

        ; using (var scope = _serviceProvider.CreateScope())
        {
            try
            {
                var voicebankLanguages = await _context.VoicebankLanguages.ToListAsync();
                var voicebankTypes = await _context.VoicebankTypes.ToListAsync();

                var searchRequest = new SearchRepositoriesRequest
                {
                    Topic = "visingers",
                    SortField = RepoSearchSort.Updated
                };
                var repositories = new List<Repository>();
                var result = await _gitHubClient.Search.SearchRepo(searchRequest);
                repositories.AddRange(result.Items);
                var singersToRemove = new List<Singer>();
                var singersCount = _context.Singers.Count();

                for (int i = 0; i < singersCount; i += 1000)
                {
                    var singers = await _context.Singers.Skip(i).Take(singersCount - i >= 1000 ? 1000 : singersCount - i).Include(singer => singer.Creator).ToListAsync();
                    singersToRemove.AddRange(singers.Where(singer => !repositories.Any(repo => singer.Creator.Login == repo.Owner.Login && singer.RepositoryName == repo.Name)));
                }

                _context.Singers.RemoveRange(singersToRemove);

                for (int count = result.TotalCount - 100, page = 2; count > 0; count -= 100, page++)
                {
                    searchRequest.Page = page;
                    repositories.AddRange((await _gitHubClient.Search.SearchRepo(searchRequest)).Items);
                }

                foreach (var repo in repositories)
                {
                    try
                    {
                        if (repo.Name.Contains("template"))
                        {
                            continue;
                        }

                        var updatedAt = repo.PushedAt > repo.UpdatedAt ? repo.PushedAt : repo.UpdatedAt;
                        var existingSinger = _context.Singers.Include(singer => singer.Creator).Include(singer => singer.Voicebanks).FirstOrDefault(x => x.RepositoryName == repo.Name && x.Creator.Login == repo.Owner.Login);
                        if (updatedAt != null && existingSinger != null && updatedAt <= existingSinger.UpdatedAt)
                        {
                            existingSinger.Stars = repo.StargazersCount;
                            continue;
                        }

                        if (updatedAt != null && (existingSinger == null || updatedAt > existingSinger.UpdatedAt))
                        {
                            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == repo.Owner.Login);
                            if (user == null)
                            {
                                var githubUserInfo = await _gitHubClient.User.Get(repo.Owner.Login);
                                user = new User
                                {
                                    Login = repo.Owner.Login,
                                    Name = githubUserInfo.Name,
                                    IsBlocked = false
                                };
                            }

                            var releases = await _gitHubClient.Repository.Release.GetAll(user.Login, repo.Name);
                            var files = (await _graphqlClient.SendQueryAsync<JsonElement>(new GraphQLRequest
                            {
                                Query = GetRepoQuery,
                                Variables = new
                                {
                                    owner = user.Login,
                                    name = repo.Name
                                }
                            })).Data
                            .GetProperty("repository")
                            .GetProperty("object")
                            .GetProperty("entries")
                            .EnumerateArray()
                            .Select(entry =>
                            {
                                var name = entry.GetProperty("name").GetString() ?? string.Empty;
                                var type = entry.GetProperty("type").GetString() ?? string.Empty;
                                var path = entry.GetProperty("path").GetString() ?? string.Empty;
                                var size = 0L;
                                string? text = null;
                                if (type == "blob")
                                {
                                    text = entry.GetProperty("object").GetProperty("text").GetString();
                                    size = entry.GetProperty("object").GetProperty("byteSize").GetInt64();
                                }
                                return new { Name = name, Type = type, Path = path, Size = size, Text = text };
                            });

                            var readmeFile = files.FirstOrDefault(f => f.Name.ToLower() == "readme.md");
                            var imageFile = files.FirstOrDefault(f => f.Name.ToLower() == "image.png" || f.Name.ToLower() == "image.jpg");
                            if (readmeFile == null || imageFile == null || readmeFile.Text == null || readmeFile.Size >= 2000000 || imageFile.Size >= 20000000)
                            {
                                continue;
                            }

                            var readmeRows = _profanityFilter.CensorText(readmeFile.Text).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            var sections = GetSections(readmeRows);
                            var descriptionSection = sections.FirstOrDefault();

                            if (descriptionSection == null)
                            {
                                continue;
                            }

                            var generalInfoSection = sections.Skip(1).FirstOrDefault();
                            var groupsSection = sections.FirstOrDefault(section => section.Name.ToLower() == groupsSectionName);
                            var videosSection = sections.FirstOrDefault(section => section.Name.ToLower() == videosSectionName);
                            var termsOfUseSection = sections.FirstOrDefault(section => section.Name.ToLower() == termsOfUseSectionName);
                            var termsOfUseSectionIndex = termsOfUseSection == null ? -1 : sections.IndexOf(termsOfUseSection);
                            var voicebankSections = sections.Skip(2).Where(section => !sectionNames.Contains(section.Name.ToLower())).ToList();
                            var voicebanks = new List<Voicebank>();
                            foreach (var voicebankSection in voicebankSections)
                            {
                                var voicebankDescription = string.Join("\n", voicebankSection.Content.Where(row => !row.StartsWith("-")));
                                var parsedLanguages = voicebankSection.Content.FirstOrDefault(row => row.StartsWith("- Languages:"))?.Replace("- Languages:", "").Split(", ").Select(lang => lang.Trim().ToLower()) ?? [];
                                var parsedType = voicebankSection.Content.FirstOrDefault(row => row.StartsWith("- Type:"))?.Replace("- Type:", "").Trim().ToLower();
                                var languages = voicebankLanguages.Where(lang => parsedLanguages.Contains(lang.Name) || parsedLanguages.Contains(lang.FullName)).Distinct().ToList();
                                var type = voicebankTypes.FirstOrDefault(type => type.Name == parsedType);
                                if (languages == null || type == null || languages.Count == 0)
                                {
                                    continue;
                                }

                                var lastRelease = releases
                                    .Where(release => release.Name.StartsWith(voicebankSection.Name))
                                    .OrderBy(release => new string(release.Name.Replace(voicebankSection.Name, string.Empty).Where(char.IsDigit).ToArray()))
                                    .FirstOrDefault();
                                if (lastRelease == null)
                                {
                                    if (voicebankSections.Count == 1)
                                    {
                                        lastRelease = releases.OrderBy(release => new string(release.Name.Where(char.IsDigit).ToArray())).FirstOrDefault();
                                        if (lastRelease == null)
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                var releaseArchive = lastRelease.Assets.FirstOrDefault(asset => asset.Name.EndsWith(".zip"));
                                if (releaseArchive == null)
                                {
                                    continue;
                                }
                                var releaseSamples = lastRelease.Assets.Where(asset => asset.Name.EndsWith(".mp3") || asset.Name.EndsWith(".wav"));

                                voicebanks.Add(new Voicebank { Type = type, Languages = languages, SampleUrls = releaseSamples.Select(sample => sample.BrowserDownloadUrl).ToList(), Url = releaseArchive.BrowserDownloadUrl, Name = voicebankSection.Name, Description = { { "en", voicebankDescription } } });
                            }

                            var description = string.Join('\n', descriptionSection.Content.Where(row => !row.StartsWith('!') && !row.StartsWith('[')));
                            var generalInfo = generalInfoSection?.Content.Where(row => row.StartsWith('-') && row.Contains(':')).Select(row => row.Trim('-', ' ')).ToList() ?? [];
                            var termsOfUse = termsOfUseSection?.Content.Where(row => row.StartsWith('-') && row.Contains(':')).Select(row => row.Trim('-', ' ')).ToList() ?? [];

                            var parsedTags = repo.Topics
                                .Where(topic => topic.ToLower() != "visingers")
                                .Select(topic => topic.ToLower().Replace("visingers-", string.Empty))
                                .Where(topic =>
                                    !voicebankLanguages.Any(lang => lang.Name == topic || lang.FullName == topic)
                                    && !voicebankTypes.Any(type => type.Name == topic)
                                    && topic != user.Login.ToLower()
                                    && topic != user.Name?.ToLower()
                                    && topic != descriptionSection.Name.ToLower()
                                    && !descriptionSection.Name.ToLower().Split().Contains(topic))
                                .Distinct();

                            var tags = _context.Tags.Where(tag => parsedTags.Contains(tag.Name)).ToList();
                            tags.AddRange(parsedTags.Where(tag => !tags.Any(foundTag => foundTag.Name == tag)).Select(tag => new Tag { Name = tag }));

                            var pattern = YoutubeRegex;

                            var videoUrls = videosSection?.Content
                                .SelectMany(input => YoutubeRegex.Matches(input).Select(match => match.Groups[1].Value))
                                .ToList() ?? [];

                            var galleryDir = files.FirstOrDefault(file => file.Name.ToLower() == "gallery" && file.Type == "tree");
                            var imageUrls = new List<string>();

                            var rawUrl = GithubRawUrl + "/" + repo.Owner.Login + "/" + repo.Name + "/" + repo.DefaultBranch + "/";

                            if (galleryDir != null)
                            {
                                imageUrls.AddRange((await _gitHubClient.Repository.Content.GetAllContents(user.Login, repo.Name, galleryDir.Name)).Where(file => file.Name.EndsWith(".png") || file.Name.EndsWith(".jpg")).Select(file => file.DownloadUrl));
                            }

                            var singer = new Singer
                            {
                                Id = existingSinger?.Id ?? 0,
                                AvatarUrl = rawUrl + "/" + imageFile.Path,
                                RepositoryName = repo.Name,
                                Name = descriptionSection.Name,
                                SiteUrl = repo.Homepage,
                                Details = { { "en", new SingerDetails { Description = description, GeneralInfo = generalInfo, TermsOfUse = termsOfUse } } },
                                Creator = user,
                                UpdatedAt = updatedAt.Value,
                                CreatedAt = repo.CreatedAt,
                                Stars = repo.StargazersCount,
                                Voicebanks = voicebanks,
                                Tags = tags,
                                VideoUrls = videoUrls,
                                ImageUrls = imageUrls
                            };

                            foreach (var file in files)
                            {
                                var fileNameArr = file.Name.Split('.');
                                if (file.Text == null || fileNameArr.Length != 3 || fileNameArr[0].ToLower() != "readme" || fileNameArr[2].ToLower() != "md")
                                {
                                    continue;
                                }

                                var translationReadmeRows = _profanityFilter.CensorText(file.Text).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                var translationSections = GetSections(translationReadmeRows);

                                var translationDescriptionSection = translationSections.FirstOrDefault();

                                if (translationDescriptionSection == null)
                                {
                                    continue;
                                }

                                var translationGeneralInfoSection = translationSections.Skip(1).FirstOrDefault();
                                var translationTermsOfUseSection = termsOfUseSectionIndex == -1 ? null : translationSections[termsOfUseSectionIndex];

                                var translatedDescription = string.Join('\n', translationDescriptionSection.Content.Where(row => !row.StartsWith('!') && !row.StartsWith('[')));
                                var translatedGeneralInfo = translationGeneralInfoSection?.Content.Where(row => row.StartsWith('-') && row.Contains(':')).Select(row => row.Trim('-', ' ')).ToList() ?? [];
                                var translatedTermsOfUse = translationTermsOfUseSection?.Content.Where(row => row.StartsWith('-') && row.Contains(':')).Select(row => row.Trim('-', ' ')).ToList() ?? [];

                                singer.Details[fileNameArr[1]] = new SingerDetails { Description = translatedDescription, GeneralInfo = translatedGeneralInfo, TermsOfUse = translatedTermsOfUse };
                                foreach (var voicebank in singer.Voicebanks)
                                {
                                    var voicebankSection = translationSections.FirstOrDefault(section => section.Name.ToLower() == voicebank.Name.ToLower());
                                    if (voicebankSection == null)
                                    {
                                        continue;
                                    }

                                    var translatedVoicebankDescription = string.Join("\n", voicebankSection.Content.Where(row => !row.StartsWith("-")));

                                    voicebank.Description[fileNameArr[1]] = translatedVoicebankDescription;
                                }

                            }

                            if (existingSinger == null)
                            {
                                await _context.Singers.AddAsync(singer);
                            }
                            else
                            {
                                existingSinger.Voicebanks = singer.Voicebanks;
                                existingSinger.Tags = singer.Tags;
                                _context.Entry(existingSinger).CurrentValues.SetValues(singer);

                            }

                            await _context.SaveChangesAsync();
                        }
                    }

                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while parsing GitHub repository.");
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GitHub parser service error.");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
    }
}
