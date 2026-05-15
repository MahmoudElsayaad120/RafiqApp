using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Services.Abstractions
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleDto>> GetDoctorArticlesAsync(int doctorId);
        Task AddArticleAsync(CreateArticleDto articleDto, int doctorId);
        Task DeleteArticleAsync(int id);
    }
}
