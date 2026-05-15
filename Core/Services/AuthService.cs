using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Constants;
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
using Persistence.Identity;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services.Abstractions;
using Shared;

namespace Services
{
    public class AuthService


        : IAuthService
    {
        private readonly RafiqIdentityDbContext _context;
        private readonly RafiqDbContext _context2;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<JwtOptions> _options;
        private readonly IMapper _mapper;

        public AuthService(UserManager<AppUser> userManager, IOptions<JwtOptions> options, IMapper mapper, RafiqIdentityDbContext context, RafiqDbContext context2,
             IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _options = options;
            _mapper = mapper;
            _context = context;
            _context2 = context2;
            _configuration = configuration;
            _roleManager = roleManager;
        }


        public async Task<UserResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) 
                throw new Exception();

            var flag = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!flag) 
                throw new Exception();

            return new UserResultDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user)
            };


            //var user = await _context.Users
            //    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            //if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            //{
            //    throw new UnauthorizedAccessException("Invalid email or password");
            //}

            //var token = GenerateJwtToken(user.Id, user.Email, user.Role);
            //var expiresAt = DateTime.UtcNow.AddMinutes(
            //    int.Parse(_configuration["JwtOptions:DurationInDays"] ?? "5")
            //);

            //return new AuthResponseDto
            //{
            //    Token = "This Will be Token",
            //    Email = user.Email,
            //    Name = user.DisplayName,
            //    Role = user.Role,
            //    ExpiresAt = expiresAt
            //};
        }

        public async Task<UserResultDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = new AppUser()
            {
                DisplayName = registerDto.Name,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };
            // create user
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (registerDto.Role == Roles.Patient)
            {
                // create role if is not exist
                if (!await _roleManager.RoleExistsAsync(Roles.Patient))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Roles.Patient));
                }

                // assign user to role
                await _userManager.AddToRoleAsync(user, Roles.Patient);

                Patient patient = new Patient()
                {
                    Name = registerDto.Name,
                    Age = registerDto.Age,
                    Gender = registerDto.Gender,
                    userId = user.Id
                };

                await _context2.Patients.AddAsync(patient);

            }
            // doctor module
            else if (registerDto.Role == Roles.Doctor)
            {
                // create role if is not exist
                if (!await _roleManager.RoleExistsAsync(Roles.Doctor))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Roles.Doctor));
                }

                // assign user to role
                await _userManager.AddToRoleAsync(user, Roles.Doctor);

                Doctor doctor = new Doctor()
                {
                    userId = user.Id,
                    FullName = registerDto.Name,
                    Email = registerDto.Email,
                    Description = registerDto.Description,
                    Price = registerDto.Price ?? 0,
                    Gender = registerDto.Gender


                };

                await _context2.Doctors.AddAsync(doctor);
            }

            await _context2.SaveChangesAsync();

            if (!result.Succeeded)
            {
                var errors =  result.Errors.Select(error => error.Description);
                throw new Exception(errors.ToString());
            }
            return new UserResultDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user)
            };

            //// Check if email already exists
            //if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            //{
            //    throw new InvalidOperationException("Email already exists");
            //}

            //// Hash password
            ////var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            //// Create user
            //var user = new AppUser
            //{
            //    DisplayName = registerDto.Name,
            //    Email = registerDto.Email
            //};

            //var createdUser = await _userManager.CreateAsync(user, registerDto.Password);

            //await _context.SaveChangesAsync();

            //// Create role-specific entity
            //if (registerDto.Role == "Patient")
            //{
            //    var patient = new Patient
            //    {
            //        //UserId = user.Id,
            //        Age = registerDto.Age,
            //        Gender = registerDto.Gender
            //    };

            //    // add patient in db
            //    _context2.Patients.Add(patient);

            //    // add role to user
            //    // check role
            //    if (!await _roleManager.RoleExistsAsync("Patient"))
            //        {
            //            await _userManager.CreateAsync(new AppUser { Email = "Patient" });
            //        }
            //_userManager.AddToRoleAsync(user, "Patient");
            //}
            //else if (registerDto.Role == "Doctor")
            //{
            //    if (string.IsNullOrEmpty(registerDto.Specialization))
            //        throw new InvalidOperationException("Specialization is required for doctors");

            //    var doctor = new Doctor
            //    {
            //        //UserId = user.Id,
            //        Specialization = registerDto.Specialization,
            //        Description = registerDto.Description,
            //        Price = registerDto.Price ?? 0
            //    };
            //    _context2.Doctors.Add(doctor);
            //}

            //await _context.SaveChangesAsync();

            //// Generate token
            //var token = GenerateJwtToken(user.Id, user.Email, user.Role);
            //var expiresAt = DateTime.UtcNow.AddMinutes(
            //    int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60")
            //);

            //return new AuthResponseDto
            //{
            //    Token = token,
            //    Email = user.Email,
            //    Name = user.Name,
            //    //    Role = user.Role,
            //    //    ExpiresAt = expiresAt
            //    //};
            //}

        }

        public async Task<string> GenerateJwtTokenAsync(AppUser user)  //int userId, string email, string role
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }


            var SecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtOptions:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtOptions:Issuer"],
                audience: _configuration["JwtOptions:Audience"],
                claims: authClaims,
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtOptions:DurationInDays"])),
                signingCredentials: new SigningCredentials(SecretKey, SecurityAlgorithms.HmacSha256Signature)

                );
            return new JwtSecurityTokenHandler().WriteToken(token);







            //    var jwtSettings = _configuration.GetSection("JwtSettings");
            //    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            //    var issuer = jwtSettings["Issuer"] ?? "RafiqAPI";
            //    var audience = jwtSettings["Audience"] ?? "RafiqUsers";

            //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //    var claims = new[]
            //    {
            //    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            //    new Claim(ClaimTypes.Email, email),
            //    new Claim(ClaimTypes.Role, role)
            //};

            //    var token = new JwtSecurityToken(
            //        issuer: issuer,
            //        audience: audience,
            //        claims: claims,
            //        expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60")),
            //        signingCredentials: credentials
            //    );

            //    return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<UserResultDto> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) throw new UserNotFoundException(email);
            return new UserResultDto
            {
            };
        }


        //public Task<string> GenerateJwtTokenAsync(int userId, string email, string role)
        //{
        //    throw new NotImplementedException();
        ////}
    }
}
