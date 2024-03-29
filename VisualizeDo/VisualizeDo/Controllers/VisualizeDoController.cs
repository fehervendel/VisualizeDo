﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisualizeDo.Context;
using VisualizeDo.Models;
using VisualizeDo.Models.DTOs;
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
        try
        {
            var user = await _userRepository.GetByEmail(email);
            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting user");
            return NotFound("Error getting user");
        }
    }

    [HttpPost("AddBoard")]
    public async Task<IActionResult> AddBoard(string email, string name)
    {
        try
        {
            var user = await _userRepository.GetByEmail(email);
            if (user == null)
            {
                return NotFound("User not found");
            }
            Board boardToAdd = new Board
            {
                Name = name,
                UserId = user.Id,
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
    public async Task<IActionResult> GetBoardById(int id)
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
            Board board = await _boardRepository.GetById(id);
            if (board != null)
            {
               await _boardRepository.DeleteById(id);
               return Ok($"Board with id: {id} has been deleted!"); 
            }
            else
            {
                return NotFound($"Board with id {id} not found!");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting board:(");
            return StatusCode(500, "Internal server Error");
        }
    }

    [HttpPut("ChangeBoardName")] //, Authorize(Roles = "Admin")
    public async Task<IActionResult> ChangeBoardName(int id, string newName)
    {
        try
        {
            var board = await _boardRepository.GetById(id);
            if (board == null)
            {
                return NotFound($"Board with id {id} not found");
            } 
            await _boardRepository.ChangeBoardName(board, newName);
            return Ok(board);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error changing board name:(");
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
                return NotFound($"Board with id {boardId} not found");
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
            return NotFound($"Board with id {boardId} not found");
        }
    }
    
    [HttpPost("AddLists")]
    public async Task<IActionResult> AddLists(int boardId, [FromBody] List<string> names)
    {
        try
        {
            var board = await _boardRepository.GetById(boardId);
            if (board == null)
            {
                return NotFound($"Board with id {boardId} not found");
            }

            List<List> listsToAdd = new List<List>();
            
            foreach (string name in names)
            {
                List listToAdd = new List
                            {
                                Name = name,
                                BoardId = boardId,
                                Board = board
                            };
                listsToAdd.Add(listToAdd);
            }
            
            await _listRepository.AddLists(listsToAdd);
            
            return Ok(listsToAdd);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding new lists :("); 
            return StatusCode(500, "Internal Server Error");
        }
    }
    
    [HttpGet("GetListById")]//, Authorize(Roles = "User, Admin")
    public async Task<IActionResult> GetListById(int id)
    {
        
        try
        {
            var list = await _listRepository.GetById(id);
            if (list != null)
            {
                return Ok(list);
            }
            else
            {
                throw new Exception($"List with id {id} not found");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting users");
            return NotFound("Error getting users");
        }
    }
    
    [HttpGet("GetListsByBoardId")]//, Authorize(Roles = "User, Admin")
    public async Task<IActionResult> GetListByBoardId(int id)
    {
        try
        {
            var lists = await _listRepository.GetByBoardId(id);
            if (lists != null || lists.Count != 0)
            {
                 return Ok(lists);
            }
            else
            {
                throw new Exception($"Board with id {id} not found");
            }
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
            List list = await _listRepository.GetById(id);
            if (list != null)
            {
                await _listRepository.DeleteById(id);
                return Ok($"List with id: {id} has been deleted!");
            }
            else
            {
                return NotFound("Error deleting list");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting list:(");
            return StatusCode(500, "Internal server Error");
        }
    }
    
    [HttpPost("AddCard")]
    public async Task<IActionResult> AddCard([FromBody] AddCard card)
    {
        try
        {
            var list = await _listRepository.GetById(card.ListId);
            if (list == null)
            {
                return NotFound($"List with id {card.ListId} not found");
            }
            Card cardToAdd = new Card
            {
                Title = card.Title,
                Description = card.Description,
                Priority = card.Priority,
                Size = card.Size,
                ListId = card.ListId,
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
    public async Task<IActionResult> GetCardById(int id)
    {
        try
        {
            var card = await _cardRepository.GetById(id);
            if (card != null)
            {
                return Ok(card);
            }
            else
            {
                return NotFound($"Card with id {id} not found");
            }
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
            var list = _listRepository.GetById(listId);
            var card = await _cardRepository.GetById(cardId);
            if (card != null && list != null)
            {
                return Ok(card);
            }
            else
            {
                return NotFound($"Card with id {cardId} or list with id {listId} not found");
            }
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Error changing List of Card");
            return NotFound($"Card with id {cardId} or list with id {listId} not found");
        }
    }
    
    [HttpPut("ChangeListName")] //, Authorize(Roles = "Admin, User")
    public async Task<IActionResult> ChangeListName(int listId, string newName)
    {
        try
        {
            List list  = await _listRepository.GetById(listId);
            if (list != null)
            {
               await _listRepository.ChangeListName(listId, newName);
               var listWithNewName = await _listRepository.GetById(listId);
               return Ok(listWithNewName); 
            }
            else
            {
                return NotFound($"List with id {listId} is not found!");
            }
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Error changing List name");
            return StatusCode(500, "Internal server Error");
        }
    }
    
    [HttpDelete("DeleteCardById")]//, Authorize(Roles = "Admin")
    public async Task<IActionResult> DeleteCardById(int id)
    {
        try
        {
            Card card = await _cardRepository.GetById(id);
            if (card != null)
            {
               await _cardRepository.DeleteById(id);
               return Ok($"Card with id: {id} has been deleted!"); 
            }
            else
            {
                return NotFound($"Card with id {id} not found");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting card:(");
            return StatusCode(500, "Internal server Error");
        }
    }
    
    [HttpDelete("DeleteUserById")]//, Authorize(Roles = "Admin")
    public async Task<IActionResult> DeleteUserById(int id)
    {
        try
        {
            User? userToDelete = await _userRepository.GetById(id);
            if (userToDelete != null)
            {
                await _userRepository.Delete(id);
                return Ok($"User with id: {id} has been deleted!");
            }
            else
            {
                return NotFound($"User with id {id} not found");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting user:(");
            return StatusCode(500, "Internal server Error");
        }
    }

    [HttpPut("EditCard")]//, Authorize(Roles = "Admin")
    public async Task<IActionResult> EditCard([FromBody] EditCard editCard)
    {
        try
        {
            Card cardToEdit = await _cardRepository.GetById(editCard.Id);
            if (cardToEdit != null)
            {
                await _cardRepository.EditCard(editCard.Id, editCard.Title, editCard.Description, editCard.Priority, editCard.Size);
                return Ok("Card has been successfully updated!");
            }
            else
            {
                return NotFound($"Card with id {editCard.Id} not found");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error editing card:(");
            return StatusCode(500, "Internal server error");
        }
    }
}