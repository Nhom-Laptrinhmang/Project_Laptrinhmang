/*namespace ChatApp.Shared.Network
{
    public enum Protocol
    {
        Connect,    
        Disconnect, 
        Message     
    }
}
*/
namespace ChatApp.Shared.Network
{
    public enum Protocol
    {
        Connect,
        Disconnect,
        Message,        // Dùng cho chat Public công khai
        PrivateMessage, // ← THÊM MỚI: Dùng cho chat Private riêng tư
        Reply,
        Forward,
        UpdateAvatar
    }

    public class ChatPacket
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string? ReceiverId { get; set; } // ← THÊM MỚI: ID người nhận (null/trống nếu là chat Public)
        public string Content { get; set; } = string.Empty;
        public string? ReplyToMessageId { get; set; }
        public bool IsForwarded { get; set; } = false;
        public string AvatarBase64 { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
