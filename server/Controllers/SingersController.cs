using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Octokit;
using ViSingers.Server;
using ViSingers.Server.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ViSingers.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SingersController : Controller
{
    private readonly ApplicationContext _context;

    public SingersController(ApplicationContext context)
    {
        _context = context;
    }

    private static VoicebankView MapVoicebankToView(Voicebank voicebank)
    {
        return new VoicebankView
        {
            Name = voicebank.Name,
            Description = voicebank.Description,
            Type = voicebank.Type.Name,
            Languages = voicebank.Languages.Select(x => x.Name).ToList(),
            Url = voicebank.Url,
            SampleUrls = voicebank.SampleUrls
        };
    }

    private static SingerView MapSingerToView(Singer singer, bool isFull = false)
    {
        return new SingerView
        {
            RepositoryName = singer.RepositoryName,
            Name = singer.Name,
            AvatarUrl = singer.AvatarUrl,
            CreatorName = singer.Creator.Name ?? singer.Creator.Login,
            CreatorLogin = singer.Creator.Login,
            Stars = singer.Stars,
            SiteUrl = singer.SiteUrl,
            Tags = singer.Tags.Select(tag => tag.Name).ToList(),
            VoicebankLanguages = singer.Voicebanks.SelectMany(s => s.Languages).Select(tag => tag.Name).Distinct().ToList(),
            VoicebankTypes = singer.Voicebanks.Select(s => s.Type).Select(tag => tag.Name).Distinct().ToList(),
            Voicebanks = isFull ? singer.Voicebanks.Select(MapVoicebankToView).ToList() : [],
            Details = isFull ? singer.Details : [],
            ImageUrls = isFull ? singer.ImageUrls : [],
            VideoUrls = isFull ? singer.VideoUrls : []
        };
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SingerView>>> Get(string name = null,
    [FromQuery] List<string>? tags = null, [FromQuery] List<string>? types = null, [FromQuery] List<string>? languages = null,
    string sort = "popular", int page = 1, int count = 40)
    {
        if (page < 1 || page < 1 || page > 100)
        {
            return BadRequest("Invalid page number or page size.");
        }

        var totalCount = await _context.Singers.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)page);

        var singers = _context.Singers
            .Skip((page - 1) * count)
            .Take(count)
            .Include(singer => singer.Creator)
            .Include(singer => singer.Tags)
            .Include(singer => singer.Voicebanks)
            .ThenInclude(voicebank => voicebank.Type)
            .Include(singer => singer.Voicebanks)
            .ThenInclude(voicebank => voicebank.Languages)
        .AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            singers = singers.Where(s => s.Name.ToLower().Contains(name.ToLower()));
        }

        if (tags != null && tags.Any())
        {
            singers = singers.Where(s => s.Tags.Any(tag => tags.Contains(tag.Name)));
        }

        if (types != null && types.Any())
        {
            singers = singers.Where(s => s.Voicebanks.Any(vb => types.Contains(vb.Type.Name)));
        }
        if (languages != null && languages.Any())
        {
            singers = singers.Where(s => s.Voicebanks.Any(vb => vb.Languages.Any(lang => languages.Contains(lang.Name))));
        }
        switch (sort)
        {
            case "popular":
                singers = singers.OrderByDescending(s => s.Stars);
                break;
            case "voicebank-count":
                singers = singers.OrderByDescending(s => s.Voicebanks.Count());
                break;
            case "recently-updated":
                singers = singers.OrderByDescending(s => s.UpdatedAt);
                break;
            case "new":
                singers = singers.OrderByDescending(s => s.CreatedAt);
                break;
            case "old":
                singers = singers.OrderBy(s => s.CreatedAt);
                break;
        }

        var singerViews = (await singers.ToListAsync()).Select(singer => MapSingerToView(singer, false)).ToList();

        var response = new
        {
            PageNumber = page,
            PageSize = count,
            TotalPages = totalPages,
            TotalCount = totalCount,
            Items = singerViews
        };

        return Ok(response);
    }

    [HttpGet("{userLogin}/{repositoryName}")]
    public async Task<ActionResult<SingerView>> Get(string userLogin, string repositoryName)
    {
        var singer = await _context.Singers
            .Include(singer => singer.Creator)
            .Include(singer => singer.Tags)
            .Include(singer => singer.Voicebanks)
            .ThenInclude(voicebank => voicebank.Type)
            .Include(singer => singer.Voicebanks)
            .ThenInclude(voicebank => voicebank.Languages)
            .FirstOrDefaultAsync(s => s.Creator.Login == userLogin && s.RepositoryName == repositoryName);
        if (singer == null)
            return NotFound();
        return new ObjectResult(MapSingerToView(singer, true));
    }
}
