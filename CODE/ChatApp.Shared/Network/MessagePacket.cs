using System;

namespace ChatApp.Shared.Network
{
    public class MessagePacket
    {
        public Protocol Type { get; set; }
        
        // Payload chứa dữ liệu thực sự (ví dụ: chuỗi JSON của ChatMessage)
        public string Payload { get; set; } 
    }
}
