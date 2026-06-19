using System;
using System.Windows.Forms;

namespace ChatApp.Client.Features
{
    public static class ReplyHandler
    {
        private static string _selectedSender = string.Empty;
        private static string _selectedContent = string.Empty;
        private static bool _isReplying = false;

        public static void SetReplyTarget(string sender, string content, Label lblQuoteDisplay, Panel pnlReplyBar)
        {
            _selectedSender = sender;
            _selectedContent = content;
            _isReplying = true;

            if (lblQuoteDisplay != null && pnlReplyBar != null)
            {
                lblQuoteDisplay.Text = $"Replying to @{sender}: \"{content}\"";
                pnlReplyBar.Visible = true;
            }
        }

        public static void CancelReply(Label lblQuoteDisplay, Panel pnlReplyBar)
        {
            _selectedSender = string.Empty;
            _selectedContent = string.Empty;
            _isReplying = false;

            if (lblQuoteDisplay != null && pnlReplyBar != null)
            {
                lblQuoteDisplay.Text = string.Empty;
                pnlReplyBar.Visible = false;
            }
        }

        public static bool IsReplying() => _isReplying;

        public static string FormatReplyMessage(string sender, string currentMsg)
        {
            if (!_isReplying) return currentMsg;
            return $"[Reply to @{_selectedSender}: {_selectedContent}]\n↳ {currentMsg}";
        }
    }
}
