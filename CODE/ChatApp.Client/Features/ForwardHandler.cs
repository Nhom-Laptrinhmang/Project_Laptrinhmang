using System;
using ChatApp.Shared.Network;

namespace ChatApp.Client.Features
{
    public static class ForwardHandler
    {
        public static MessagePacket CreateForwardPacket(string currentUsername, string selectedText, string targetRecipient, string avatarBase64)
        {
            return new MessagePacket
            {
                Type = (targetRecipient == "All") ? Protocol.Message : Protocol.PrivateMessage,
                Sender = currentUsername,
                Content = $"[Chuyển tiếp]: {selectedText}",
                Receiver = targetRecipient,
                AvatarBase64 = avatarBase64,
                MessageId = Guid.NewGuid().ToString()
            };
        }
    }
}
