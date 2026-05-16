using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ArticleListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string ImagePath { get; set; }
        public string Category { get; set; }
        public string TimeAgo { get; set; } // مثل: "منذ يومين"
    }
}
