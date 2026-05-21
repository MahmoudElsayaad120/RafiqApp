using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SavedArticle : BaseEntity<int>
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
