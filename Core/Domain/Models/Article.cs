using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Identity;

namespace Domain.Models
{
    public class Article : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public bool IsPublished { get; set; } = true;



        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;

    }
}
