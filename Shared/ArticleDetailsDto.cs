using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ArticleDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; }
        public string Category { get; set; }
        public string TimeAgo { get; set; }

        // بيانات الدكتور الكاتب
        public string DoctorName { get; set; }
        public string? DoctorImagePath { get; set; }

        // مقالات ذات صلة (نفس التصنيف مثلاً)
        public List<ArticleListDto> RelatedArticles { get; set; } = new List<ArticleListDto>();
    }
}
