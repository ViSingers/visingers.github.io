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
public class TagsController : Controller
{
    private readonly ApplicationContext _context;

    public TagsController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> Get()
    {
        var tags = await _context.Tags.Select(tag => tag.Name).ToListAsync();

        return Ok(tags);
    }
}
