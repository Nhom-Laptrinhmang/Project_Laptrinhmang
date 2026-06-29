/*
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
*/
using System;

namespace ChatApp.Shared.Network
{
    public class MessagePacket
    {
        public Protocol Type { get; set; }

        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        public string Sender { get; set; } = "";

        public string Receiver { get; set; } = "";

        public string Content { get; set; } = "";

        public string AvatarBase64 { get; set; } = "";

        public string ReplyToId { get; set; } = "";

        public string ReplyToContent { get; set; } = "";

        public string Payload { get; set; } = "";
        public string TimeSent { get; set; } = string.Empty; // Thuộc tính lưu thời gian gửi (HH:mm)

    }
}