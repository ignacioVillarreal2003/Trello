using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Filters;
using TrelloApi.Domain.Entities.User;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.User.DTO;
using TrelloApi.Infrastructure.Authentication;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : BaseController
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IJwt _jwt;

    public UserController(ILogger<UserController> logger, IUserService userService, IJwt jwt)
    {
        _logger = logger;
        _userService = userService;
        _jwt = jwt;
    }
    
    [HttpGet]
    [RequireAuthentication]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            List<OutputUserDto> users = await _userService.GetUsers(UserId);
            _logger.LogDebug("Retrieved {Count} users.", users.Count);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet("username/{username}")]
    [RequireAuthentication]
    public async Task<IActionResult> GetUsersByUsername(string username)
    {
        try
        {
            List<OutputUserDto> users = await _userService.GetUsersByUsername(UserId, username);
            _logger.LogDebug("Retrieved {Count} users for username {Username}", users.Count, username);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for username {Username}", username);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet("board/{boardId:int}")]
    [RequireAuthentication]
    public async Task<IActionResult> GetUsersByBoardId(int boardId)
    {
        try
        {
            List<OutputUserDto> users = await _userService.GetUsersByBoardId(UserId, boardId);
            _logger.LogDebug("Retrieved {Count} users.", users.Count);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpGet("task/{taskId:int}")]
    [RequireAuthentication]
    public async Task<IActionResult> GetUsersByTaskId(int taskId)
    {
        try
        {
            List<OutputUserDto> users = await _userService.GetUsersByTaskId(UserId, taskId);
            _logger.LogDebug("Retrieved {Count} users.", users.Count);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
    {
        try
        {
            OutputUserDto? user = await _userService.RegisterUser(registerUserDto);
            if (user == null)
            {
                _logger.LogError("Failed to register user {Email}", registerUserDto.Email);
                return BadRequest(new { message = "Failed to register user." });
            }

            string token = _jwt.GenerateToken(user.Id);
            
            _logger.LogInformation("User {UserId} register", user.Id);
            return CreatedAtAction(nameof(GetUsersByUsername), new { username = user.Username }, new { token, user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error register user {Email}", registerUserDto.Email);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost("login-user")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        try
        {
            OutputUserDto? user = await _userService.LoginUser(loginUserDto);
            if (user == null)
            {
                _logger.LogError("Failed to login user {Email}", loginUserDto.Email);
                return BadRequest(new { message = "Failed to login user." });
            }

            string token = _jwt.GenerateToken(user.Id);
            
            _logger.LogInformation("User {UserId} login", user.Id);
            return Ok (new { token, user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error login user {Email}", loginUserDto.Email);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPut]
    [RequireAuthentication]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            OutputUserDto? user = await _userService.UpdateUser(UserId, updateUserDto);
            if (user == null)
            {
                _logger.LogDebug("User {UserId} not found for update", UserId);
                return NotFound(new { message = "User not found." });
            }

            _logger.LogInformation("User {UserId} updated", UserId);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete]
    [RequireAuthentication]
    public async Task<IActionResult> DeleteUser()
    {
        try
        {
            OutputUserDto? user = await _userService.DeleteUser(UserId);
            if (user == null)
            {
                _logger.LogDebug("User {UserId} not found for deletion", UserId);
                return NotFound(new { message = "User not found." });
            }

            _logger.LogInformation("User {UserId} deleted", UserId);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}
