using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ViSingers.Server;
using ViSingers.Server.Models;

namespace ViSingers.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller
{
    private readonly ApplicationContext _context;

    public UsersController(ApplicationContext context)
    {
        _context = context;
    }

    private static SingerView MapSingerToView(Singer singer)
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
            VoicebankTypes = singer.Voicebanks.Select(s => s.Type).Select(tag => tag.Name).Distinct().ToList()
        };
    }

    private static UserView MapUserToView(User user)
    {
        return new UserView
        {
            Name = user.Name ?? user.Login,
            Login = user.Login,
            Singers = user.Singers.Select(MapSingerToView).ToList()
        };
    }

    [HttpGet("{userLogin}")]
    public async Task<ActionResult<UserView>> Get(string userLogin)
    {
        var user = await _context.Users
            .Include(user => user.Singers)
            .ThenInclude(singer => singer.Tags)
            .Include(user => user.Singers)
            .ThenInclude(singer => singer.Voicebanks)
            .ThenInclude(voicebank => voicebank.Type)
            .Include(user => user.Singers)
            .ThenInclude(singer => singer.Voicebanks)
            .ThenInclude(voicebank => voicebank.Languages)
            .FirstOrDefaultAsync(u => u.Login == userLogin);

        if (user == null)
            return NotFound();

        return new ObjectResult(MapUserToView(user));
    }
}
