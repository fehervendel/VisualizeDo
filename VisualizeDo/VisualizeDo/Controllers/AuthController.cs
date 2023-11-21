using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisualizeDo.Contracts;
using VisualizeDo.Repositories;
using VisualizeDo.Services.Authentication;
using VisualizeDo.Contracts;
using VisualizeDo.Models;
using VisualizeDo.Models.DTOs;
using VisualizeDo.Services.Authentication;

namespace VisualizeDo.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authenticationService;
    private readonly IUserRepository _userRepository;

    public AuthController(IAuthService authenticationService, IUserRepository userRepository)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authenticationService.RegisterAsync(request.Email, request.Username, request.Password, "User");
        
        if (!result.Success)
        {
            AddErrors(result);
            return BadRequest(ModelState);
        }

        User user = new User
        {
            Name = request.Username,
            EmailAddress = request.Email,
            IdentityUserId = result.IdentityUserId
        };

        await _userRepository.Add(user);

        return CreatedAtAction(nameof(Register), new RegistrationResponse(result.Email, result.UserName));
    }
    
    [HttpPost("RegisterAdmin"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrationResponse>> RegisterAdmin(RegistrationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authenticationService.RegisterAsync(request.Email, request.Username, request.Password, "Admin");
        
        if (!result.Success)
        {
            AddErrors(result);
            return BadRequest(ModelState);
        }
        
        User user = new User
        {
            Name = request.Username,
            EmailAddress = request.Email,
            IdentityUserId = result.IdentityUserId
        };

        await _userRepository.Add(user);

        return CreatedAtAction(nameof(Register), new RegistrationResponse(result.Email, result.UserName));
    }

    private void AddErrors(AuthResult result)
    {
        foreach (var error in result.ErrorMessages)
        {
            ModelState.AddModelError(error.Key, error.Value);
        }
    }
    
    [HttpPost("Login")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authenticationService.LoginAsync(request.Email, request.Password);

        if (!result.Success)
        {
            AddErrors(result);
            return BadRequest(ModelState);
        }

        return Ok(new AuthResponse(result.Email, result.UserName, result.Token));
    }
}