using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public class JwtService: IJwtService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly string _secret;

    public JwtService(IConfiguration configuration, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _secret = configuration["Jwt:Key"];
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }
    
    public string GenerateAccessToken(int userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("UserId", userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }

    public async Task SaveRefreshToken(int userId, string refreshToken)
    {
        var user = await _userRepository.GetAsync(u => u.Id.Equals(userId));
        if (user == null) throw new Exception("User not found.");
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync();
    }

    public async Task<int?> ValidateRefreshToken(string refreshToken)
    {
        var user = await _userRepository.GetAsync(u => u.RefreshToken.Equals(refreshToken));
        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return null;
        }

        return user.Id;
    }
}