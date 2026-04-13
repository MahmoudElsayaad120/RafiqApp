using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services.Abstractions;
using Shared;
using Shared.OrderModels;

namespace Rafiq.Api.Services
{
    public class AuthService(
        UserManager<AppUser> userManager, 
        IOptions<JwtOptions> options,
        IMapper mapper
        
        ) : IAuthService
    {
        private readonly RafiqDbContext _context;
        private readonly IConfiguration _configuration;

        //public AuthService(RafiqDbContext context, IConfiguration configuration)
        //{
        //    _context = context;
        //    _configuration = configuration;
        //}


        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token = GenerateJwtToken(user.Id, user.Email, user.Role);
            var expiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60")
            );

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                ExpiresAt = expiresAt
            };
        }
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create user
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                Role = registerDto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create role-specific entity
            if (registerDto.Role == "Patient")
            {
                var patient = new Patient
                {
                    UserId = user.Id,
                    Age = registerDto.Age,
                    Gender = registerDto.Gender
                };
                _context.Patients.Add(patient);
            }
            else if (registerDto.Role == "Doctor")
            {
                if (string.IsNullOrEmpty(registerDto.Specialization))
                    throw new InvalidOperationException("Specialization is required for doctors");

                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Specialization = registerDto.Specialization,
                    Description = registerDto.Description,
                    Price = registerDto.Price ?? 0
                };
                _context.Doctors.Add(doctor);
            }

            await _context.SaveChangesAsync();

            // Generate token
            var token = GenerateJwtToken(user.Id, user.Email, user.Role);
            var expiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60")
            );

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                ExpiresAt = expiresAt
            };
        }


        public string GenerateJwtToken(int userId, string email, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "RafiqAPI";
            var audience = jwtSettings["Audience"] ?? "RafiqUsers";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
           var user = await userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<UserResultDto> GetCurrentUserAsync(string email)
        {
           var user = await  userManager.FindByEmailAsync(email);
            if (user is null) throw new UserNotFoundException(email);
            return new UserResultDto
            {
                
            };

        }

        public async Task<addressDto> GetCurrentUserAddressAsync(string email)
        {
          var user = await userManager.Users.Include(U => U.Address).FirstOrDefaultAsync(U => U.Email == email);
            if (user is null) throw new UserNotFoundException(email);
           var result = mapper.Map<addressDto>(user.Address);
            return result;
        }

        public async Task<addressDto> UpdateCurrentUserAddressAsync(addressDto address, string email)
        {
            var user = await userManager.Users.Include(U => U.Address).FirstOrDefaultAsync(U => U.Email == email);
            if (user is null) throw new UserNotFoundException(email);
            if (user.Address is not null)
            {
                user.Address.FirstName = address.FirstName;
                user.Address.LastName = address.LastName;
                user.Address.Street = address.Street;
                user.Address.City = address.Street;
                user.Address.Country = address.Country;
            }
            else 
            {
               var AddressResult = mapper.Map<Address>(address);
                user.Address = AddressResult;
            }
            return address;
        }
    }
}
