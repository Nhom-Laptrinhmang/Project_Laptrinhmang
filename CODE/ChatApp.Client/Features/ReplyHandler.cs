using ChatApp.Shared.Network;

namespace ChatApp.Client.Features
{
    public static class ReplyHandler
    {
        public static void CreateReply(MessagePacket packet, string replyToId, string replyToContent)
        {
            if (packet == null) return;

            packet.ReplyToId = replyToId;
            packet.ReplyToContent = replyToContent;
        }

        public static string FormatReplyDisplay(string sender, string currentContent, string originalContent)
        {
            if (string.IsNullOrEmpty(originalContent)) return $"{sender}: {currentContent}";
            return $"(Đang trả lời: \"{originalContent}\")\r\n👉 {sender}: {currentContent}";
        }
    }
}
