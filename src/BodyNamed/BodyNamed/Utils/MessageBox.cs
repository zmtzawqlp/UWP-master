using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;

namespace BodyNamed.Utils
{
    public static class MessageBox
    {
        public static async Task<MessageBoxResult> AskAsync(string content, string title = null, bool reverseIndex = false)
        {
            return (MessageBoxResult)(await AskAsync(content, title, reverseIndex, "确认", "取消"));
        }

        public static async Task<int> AskAsync(string content, string title = null, bool reverseIndex = false, params string[] labels)
        {
            IUICommand[] commands = labels?.Select(label => new UICommand(label)).ToArray();
            IUICommand command = await ShowAsync(content, title, reverseIndex, commands);
            return (commands == null ? 0 : Array.IndexOf(commands, command));
        }

        public static IAsyncOperation<IUICommand> ShowAsync(string content, string title = null, bool reverseIndex = false, params IUICommand[] commands)
        {
            var dialog = (title == null ? new MessageDialog(content) : new MessageDialog(content, title));
            if (commands?.Length > 0)
            {
                foreach (IUICommand command in commands)
                {
                    dialog.Commands.Add(command);
                }

                if (reverseIndex)
                {
                    dialog.CancelCommandIndex = 0;
                    dialog.DefaultCommandIndex = (uint)(commands.Length - 1);
                }
                else
                {
                    dialog.DefaultCommandIndex = 0;
                    dialog.CancelCommandIndex = (uint)(commands.Length - 1);
                }              
            }
            //dialog.Options = MessageDialogOptions.AcceptUserInputAfterDelay; // 对话框弹出后，短时间内禁止用户单击命令按钮，以防止用户的误操作
            return dialog.ShowAsync();
        }
    }

    public enum MessageBoxResult
    {
        OK,
        Cancel
    }
}
