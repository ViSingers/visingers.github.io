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
public class VoicebankLanguagesController : Controller
{
    private readonly ApplicationContext _context;

    public VoicebankLanguagesController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> Get()
    {
        var languages = await _context.VoicebankLanguages.Select(lang => lang.Name).ToListAsync();

        return Ok(languages);
    }
}
