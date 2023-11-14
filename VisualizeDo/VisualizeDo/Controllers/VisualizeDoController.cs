using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisualizeDo.Context;
using VisualizeDo.Models;

namespace VisualizeDo.Controllers;
[ApiController]
[Route("[controller]")]
public class VisualizeDoController : ControllerBase
{
    private readonly ILogger<VisualizeDoController> _logger;

    public VisualizeDoController(ILogger<VisualizeDoController> logger)
    {
        _logger = logger;
    }

    [HttpGet("GetAllTodos")]
    public async Task<ActionResult<Card>> GetAllTodos()
    {
        await using var dbContext = new VisualizeDoContext();
        var cards = await dbContext.Cards.ToListAsync();

        try
        {
            return Ok(cards);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting todos");
            return NotFound("Error getting todos");
        }
    }
}