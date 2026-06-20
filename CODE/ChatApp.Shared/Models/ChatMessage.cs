using System;

namespace ChatApp.Shared.Models
{
    public class ChatMessage
    {
        public string Sender { get; set; } 
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } 

        public ChatMessage()
        {
            Timestamp = DateTime.Now; 
        }
    }
}
