using Rafiq.Api.DTOs;
using Shared;
using Shared.OrderModels;

namespace Rafiq.Api.Services.Abstractions;

public interface IAuthService
{

    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    string GenerateJwtToken(int userId, string email, string role);

    Task<bool> CheckEmailExistsAsync(string email);
    Task<UserResultDto> GetCurrentUserAsync(string email);
    Task<addressDto> GetCurrentUserAddressAsync(string email);
    Task<addressDto> UpdateCurrentUserAddressAsync(addressDto address, string email);
}
