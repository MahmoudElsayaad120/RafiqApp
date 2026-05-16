using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared
{
    public class UploadRecordDto
    {
        public string Title { get; set; }
        public IFormFile File { get; set; } // الملف الفعلي اللي جاي من الـ React
    }
}
