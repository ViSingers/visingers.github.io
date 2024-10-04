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
public class VoicebankTypesController : Controller
{
    private readonly ApplicationContext _context;

    public VoicebankTypesController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> Get()
    {
        var types = await _context.VoicebankTypes.Select(type => type.Name).ToListAsync();

        return Ok(types);
    }
}
