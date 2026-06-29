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
        PrivateMessage,     
        Reply,
        Forward,
        UpdateAvatar,
        UserList
    }

    public class ChatPacket
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ReplyToMessageId { get; set; } = string.Empty;
        public bool IsForwarded { get; set; } = false;
        public string AvatarBase64 { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
