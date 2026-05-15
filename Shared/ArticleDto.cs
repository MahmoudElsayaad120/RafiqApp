using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string TimeAge { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
    }
}
