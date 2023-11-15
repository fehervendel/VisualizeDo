using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisualizeDo.Context;
using VisualizeDo.Models;
using VisualizeDo.Repositories;

namespace VisualizeDo.Controllers;
[ApiController]
[Route("[controller]")]
public class VisualizeDoController : ControllerBase
{
    private readonly ILogger<VisualizeDoController> _logger;
    private ICardRepository _cardRepository;

    public VisualizeDoController(ILogger<VisualizeDoController> logger, ICardRepository cardRepository)
    {
        _logger = logger;
        _cardRepository = cardRepository;
    }

    [HttpGet("GetAllCards"), Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<Card>> GetAllCards()
    {
        var cards = await _cardRepository.GetAll();
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