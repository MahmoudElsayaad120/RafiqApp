using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ErrorModels
{
    public class ErrorDetails
    {
        public int statusCode { get; set; }
        public string ErrorMessage { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
