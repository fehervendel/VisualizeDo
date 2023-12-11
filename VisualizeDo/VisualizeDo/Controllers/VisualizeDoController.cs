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
    private readonly ICardRepository _cardRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly IListRepository _listRepository;
    private readonly IUserRepository _userRepository;

    public VisualizeDoController(ILogger<VisualizeDoController> logger, ICardRepository cardRepository, IBoardRepository boardRepository, IListRepository listRepository, IUserRepository userRepository)
    {
        _logger = logger;
        _cardRepository = cardRepository;
        _boardRepository = boardRepository;
        _listRepository = listRepository;
        _userRepository = userRepository;
    }

    [HttpGet("GetAllCards")]//, Authorize(Roles = "User, Admin")
    public async Task<ActionResult<Card>> GetAllCards()
    {
        var cards = await _cardRepository.GetAll();
        try
        {
            return Ok(cards);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting cards");
            return NotFound("Error getting cards");
        }
    }
    
    [HttpGet("GetUserById")]//, Authorize(Roles = "User, Admin")
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _userRepository.GetById(id);
        try
        {
            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting user");
            return NotFound("Error getting user");
        }
    }
    
    [HttpGet("GetUserByEmail")]//, Authorize(Roles = "User, Admin")
    public async Task<ActionResult<User>> GetUserByEmail(string email)
    {
        var user = await _userRepository.GetByEmail(email);
        try
        {
            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting user");
            return NotFound("Error getting user");
        }
    }

    [HttpPost("AddBoard")]
    public async Task<IActionResult> AddBoard(int userId, string name)
    {
        try
        {
            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            Board boardToAdd = new Board
            {
                Name = name,
                UserId = userId,
                User = user
            };
            
            await _boardRepository.Add(boardToAdd);
            
            return Ok(boardToAdd);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding new board :("); 
            return StatusCode(500, "Internal Server Error");
        }
    }
    
    [HttpGet("GetBoardById")]//, Authorize(Roles = "User, Admin")
    public async Task<ActionResult<User>> GetBoardById(int id)
    {
        var board = await _boardRepository.GetById(id);
        try
        {
            return Ok(board);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting board");
            return NotFound("Error getting board");
        }
    }
    
    [HttpDelete("DeleteBoardById")]//, Authorize(Roles = "Admin")
    public async Task<IActionResult> DeleteBoardById(int id)
    {
        try
        {
            await _boardRepository.DeleteById(id);
            return Ok($"Board with id: {id} has been deleted!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting board:(");
            return StatusCode(500, "Internal server Error");
        }
    }
    
    [HttpPost("AddList")]
    public async Task<IActionResult> AddList(int boardId, string name)
    {
        try
        {
            var board = await _boardRepository.GetById(boardId);
            if (boardId == null)
            {
                return NotFound("Board not found");
            }
            List listToAdd = new List
            {
                Name = name,
                BoardId = boardId,
                Board = board
            };
            
            await _listRepository.Add(listToAdd);
            
            return Ok(listToAdd);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding new list :("); 
            return StatusCode(500, "Internal Server Error");
        }
    }
    [HttpGet("GetListById")]//, Authorize(Roles = "User, Admin")
    public async Task<ActionResult<User>> GetListById(int id)
    {
        var list = await _listRepository.GetById(id);
        try
        {
            return Ok(list);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting users");
            return NotFound("Error getting users");
        }
    }
    
    [HttpGet("GetListsByBoardId")]//, Authorize(Roles = "User, Admin")
    public async Task<ActionResult<List<List>>> GetListByBoardId(int id)
    {
        var list = await _listRepository.GetByBoardId(id);
        try
        {
            return Ok(list);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting users");
            return NotFound("Error getting users");
        }
    }

    [HttpDelete("DeleteListById")]//, Authorize(Roles = "Admin")
    public async Task<IActionResult> DeleteListById(int id)
    {
        try
        {
            await _listRepository.DeleteById(id);
            return Ok($"List with id: {id} has been deleted!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting list:(");
            return StatusCode(500, "Internal server Error");
        }
    }
    
    [HttpPost("AddCard")]
    public async Task<IActionResult> AddCard(int listId, string title, string description, string priority, string size)
    {
        try
        {
            var list = await _listRepository.GetById(listId);
            if (list == null)
            {
                return NotFound("Board not found");
            }
            Card cardToAdd = new Card
            {
                Title = title,
                Description = description,
                Priority = priority,
                Size = size,
                ListId = listId,
                List = list
            };
            
            await _cardRepository.Add(cardToAdd);
            
            return Ok(cardToAdd);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding new list :("); 
            return StatusCode(500, "Internal Server Error");
        }
    }
    
    [HttpGet("GetCardById")]//, Authorize(Roles = "User, Admin")
    public async Task<ActionResult<User>> GetCardById(int id)
    {
        var card = await _cardRepository.GetById(id);
        try
        {
            return Ok(card);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting card");
            return NotFound("Error getting card");
        }
    }

    [HttpPut("ChangeCardsListById")] //, Authorize(Roles = "Admin, User")
    public async Task<IActionResult> ChangeCardsListById(int cardId, int listId)
    {
        try
        {
            await _cardRepository.ChangeList(cardId, listId);
            return Ok();
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Error changing List of Card");
            return StatusCode(500, "Internal server Error");
        }
    }
    
    [HttpDelete("DeleteCardById")]//, Authorize(Roles = "Admin")
    public async Task<IActionResult> DeleteCardById(int id)
    {
        try
        {
            await _cardRepository.DeleteById(id);
            return Ok($"Card with id: {id} has been deleted!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting card:(");
            return StatusCode(500, "Internal server Error");
        }
    }
}