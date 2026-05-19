using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ChatMessageDto
    {
        public string Sender { get; set; }
        public string MessageText { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
