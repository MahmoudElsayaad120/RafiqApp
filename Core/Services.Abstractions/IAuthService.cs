using System.Security.Principal;
using Domain.Models.Identity;
using Rafiq.Api.DTOs;
using Shared;

namespace Rafiq.Api.Services.Abstractions;

public interface IAuthService
{

    Task<UserResultDto> LoginAsync(LoginDto loginDto);
    Task<UserResultDto> RegisterAsync(RegisterDto registerDto);
   Task<string> GenerateJwtTokenAsync(AppUser user);
    Task<bool> CheckEmailExistsAsync(string email);
    Task<UserResultDto> GetCurrentUserAsync(string email);
    Task<bool> SendOtpAsync(SendOtpDto sendOtpDto); // ??? ??  OTP
    Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto); // ??? ??  OTP

}
