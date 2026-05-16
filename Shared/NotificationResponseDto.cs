using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // عشان الأيقونات (أزرق للمواعيد، أخضر للملفات، إلخ)
        public bool IsRead { get; set; }
        public string TimeAgo { get; set; }
    }
}
