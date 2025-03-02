using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.User;

namespace TrelloApi.Application.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting("fixed")]
public class UserController : BaseController
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public UserController(ILogger<UserController> logger, IUserService userService, IJwtService jwtService)
    {
        _logger = logger;
        _userService = userService;
        _jwtService = jwtService;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            List<UserResponse> users = await _userService.GetUsers();
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
    [Authorize]
    public async Task<IActionResult> GetUsersByUsername(string username)
    {
        try
        {
            List<UserResponse> users = await _userService.GetUsersByUsername(username);
            _logger.LogDebug("Retrieved {Count} users for username {Username}", users.Count, username);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for username {Username}", username);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpGet("card/{cardId:int}")]
    [Authorize]
    public async Task<IActionResult> GetUsersByCardId(int cardId)
    {
        try
        {
            List<UserResponse> users = await _userService.GetUsersByCardId(cardId);
            _logger.LogDebug("Retrieved {Count} users.", users.Count);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
    {
        try
        {
            UserResponse user = await _userService.RegisterUser(registerUserDto);
            
            string accessToken = _jwtService.GenerateAccessToken(user.Id);
            string refreshToken = _jwtService.GenerateRefreshToken();
            await _jwtService.SaveRefreshToken(user.Id, refreshToken);
            
            _logger.LogInformation("User {UserId} register", user.Id);
            return CreatedAtAction(nameof(GetUsersByUsername), new { username = user.Username }, new { accessToken, refreshToken, user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error register user {Email}", registerUserDto.Email);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost("login")]
    [EnableRateLimiting("block")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        try
        {
            UserResponse? user = await _userService.LoginUser(loginUserDto);
            if (user == null)
            {
                _logger.LogError("Failed to login user {Email}", loginUserDto.Email);
                return BadRequest(new { message = "Failed to login user." });
            }

            string accessToken = _jwtService.GenerateAccessToken(user.Id);
            string refreshToken = _jwtService.GenerateRefreshToken();
            await _jwtService.SaveRefreshToken(user.Id, refreshToken);
            
            _logger.LogInformation("User {UserId} login", user.Id);
            return Ok (new { accessToken, refreshToken, user });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Invalid user credentials for {Email}", loginUserDto.Email);
            return Unauthorized(new { message = "Invalid user credentials." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error login user {Email}", loginUserDto.Email);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var userId = await _jwtService.ValidateRefreshToken(request.RefreshToken);
        if (userId == null)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        string accessToken = _jwtService.GenerateAccessToken(userId.Value);
        string refreshToken = _jwtService.GenerateRefreshToken();
        await _jwtService.SaveRefreshToken(userId.Value, refreshToken);

        return Ok (new { accessToken, refreshToken });
    }
    
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            UserResponse? user = await _userService.UpdateUser(updateUserDto, UserId);
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
    [Authorize]
    public async Task<IActionResult> DeleteUser()
    {
        try
        {
            Boolean isDeleted = await _userService.DeleteUser(UserId);
            if (!isDeleted)
            {
                _logger.LogDebug("User {UserId} not found for deletion", UserId);
                return NotFound(new { message = "User not found." });
            }

            _logger.LogInformation("User {UserId} deleted", UserId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", UserId);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}
