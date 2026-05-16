using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Persistence;
using Services.Abstractions;
using Shared;

namespace Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;





        public ArticleService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public async Task<IEnumerable<ArticleDto>> GetDoctorArticlesAsync(int doctorId)
        {
            var baseUrl = _configuration["BaseUrl"];

            var articles = await _unitOfWork.GetRepository<Article, int>().GetAllAsync();
            return articles.Where(a => a.DoctorId == doctorId).OrderByDescending(a => a.CreateAt).Select(a => new ArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                ImagePath = $"{baseUrl}{a.ImagePath}",
                Category = a.Category,
                IsPublished = a.IsPublished,
                TimeAge = GetTimeAge(a.CreateAt)
            }).ToList();

        }
        public async Task AddArticleAsync(CreateArticleDto articleDto, int doctorId)
        {
            string savedPath = string.Empty;

            // Handle File Upload
            if (articleDto.ProfileImage != null && articleDto.ProfileImage.Length > 0)
            {
                // 1. Create a unique file name to avoid overwriting
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(articleDto.ProfileImage.FileName)}";

                // 2. Define the folder path (ensure this directory exists!)
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/articles");
                if (!Directory.Exists(uploadDirectory)) Directory.CreateDirectory(uploadDirectory);

                var fullPath = Path.Combine(uploadDirectory, fileName);

                // 3. Save the file to disk
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await articleDto.ProfileImage.CopyToAsync(stream);
                }

                // 4. Set the path for the database (relative path usually works best for web)
                savedPath = $"/uploads/articles/{fileName}";
            }

            var article = new Article
            {
                Title = articleDto.Title,
                Content = articleDto.Content,
                ImagePath = savedPath,
                Category = articleDto.Category,
                DoctorId = doctorId,
                CreateAt = DateTime.Now,
                IsPublished = true
            };

            await _unitOfWork.GetRepository<Article, int>().AddAsync(article);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteArticleAsync(int id)
        {
            var article = _unitOfWork.GetRepository<Article, int>();
            if (article != null) 
            {
                article.Delete(id);
                await _unitOfWork.SaveChangesAsync();
            }
        }
         




        private string GetTimeAge(DateTime dateTime)
        {
            var span = DateTime.Now - dateTime;
            if (span.TotalMinutes < 60) return $" {Math.Floor(span.TotalMinutes)} منذ دقيقه";
            if (span.TotalHours < 24) return $"{Math.Floor(span.TotalHours)} منذ ساعه";
            if (span.TotalHours < 30) return $"{Math.Floor(span.TotalDays)} منذ يوم ";

            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}
