using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;
using Services.Abstractions;
using Shared;

namespace Presentation
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService) => _articleService = articleService;

        [HttpGet("MyArticles")]
        public async Task<IActionResult> GetMyArticles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctorId = await GetDoctorIdFromUserIdAsync(userId!);
            var articles = await _articleService.GetDoctorArticlesAsync(doctorId);
            return Ok(articles);
        }

        [HttpPost("AddArticle")]
        public async Task<IActionResult> AddArticle([FromForm]CreateArticleDto articleDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctorId = await GetDoctorIdFromUserIdAsync(userId!);
            await _articleService.AddArticleAsync(articleDto, doctorId);
            return Ok(new { message = "Article added successfully" });
        }

        [HttpDelete("DeleteArticle/{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            await _articleService.DeleteArticleAsync(id);
            return Ok(new { message = "Article deleted successfully" });


        }

        private async Task<int> GetDoctorIdFromUserIdAsync(string userId)
        {
            using var scope = HttpContext.RequestServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RafiqDbContext>();
            var doctor = await context.Doctors.FirstOrDefaultAsync(d => d.userId == userId);
            if (doctor == null)
                throw new UnauthorizedAccessException("Doctor profile not found");
            return doctor.Id;
        }
    }
}
