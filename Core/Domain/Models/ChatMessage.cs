using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ChatMessage : BaseEntity<int>
    {
        public string Message { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty; // "Patient" or "Bot"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
