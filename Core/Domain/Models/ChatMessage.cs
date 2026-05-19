using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ChatMessage : BaseEntity<int>
    {
        public string Sender { get; set; } // "User" أو "Rafeeq"
        public string MessageText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ChatSessionId { get; set; }
        public ChatSession ChatSession { get; set; }

    }
}
