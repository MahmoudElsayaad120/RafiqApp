using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class MedicalFileResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string FormattedDate { get; set; } // لعرض التاريخ بشكل منسق (مثلاً: ديسمبر 2025)
        public string? Notes { get; set; }
    }
}
