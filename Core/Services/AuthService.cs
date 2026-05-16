using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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

            await SendOtpAsync(new SendOtpDto { Email = user.Email }); // إرسال OTP بعد التسجيل اوتوماتكيكي
            return new UserResultDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user)
            };

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


        public async Task<bool> SendOtpAsync(SendOtpDto sendOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(sendOtpDto.Email);
            if (user is null) return false;

            // 1. توليد كود عشوائي آمن ومضمون 4 أرقام دايماً
            string otpCode = RandomNumberGenerator.GetInt32(1000, 10000).ToString();

            // 2. حفظ الكود ووقت الصلاحية (60 دقيقة من الآن)
            user.OtpCode = otpCode;
            user.OtpExpiryTime = DateTime.Now.AddMinutes(60);

            await _userManager.UpdateAsync(user);

            // 3. إرسال الإيميل (اربطها هنا بخدمة الـ Email المعتمدة عندك)
            try
            {
                // _emailService.SendEmail(user.Email, "رمز التحقق", $"كود التحقق الخاص بك هو: {otpCode}");

                // للتيست في الـ Output Console
                Console.WriteLine($"================ OTP Sent to {sendOtpDto.Email}: {otpCode} ================");
                return true;
            }
            catch
            {
                return false;
            }
        } // جزء ال  OTP

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(verifyOtpDto.Email);
            if (user is null) return false;

            // التحقق من صحة الكود والوقت
            if (user.OtpCode == null ||
                user.OtpCode != verifyOtpDto.Code ||
                user.OtpExpiryTime < DateTime.Now)
            {
                return false; // الكود خطأ أو صلاحيته انتهت
            }

            // تصغير الحقول بعد التحقق الناجح لمنع استخدام الكود مرة أخرى
            user.OtpCode = null;
            user.OtpExpiryTime = null;

            // تفعيل الحساب في Identity داتا بيز
            user.EmailConfirmed = true;

            await _userManager.UpdateAsync(user);
            return true;
        } // جزء ال  OTP

    }
}
