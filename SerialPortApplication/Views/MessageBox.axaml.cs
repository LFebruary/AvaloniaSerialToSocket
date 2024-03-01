// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace SerialToSocket.AvaloniaApp.Views
{
    /// <summary>
    /// Represents a message box dialog for displaying information to the user and receiving simple responses.
    /// </summary>
    public partial class MessageBox : Window
    {
        /// <summary>
        /// Defines the available buttons for the message box.
        /// </summary>
        public enum MessageBoxButtons
        {
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel
        }

        /// <summary>
        /// Represents the possible results of the message box.
        /// </summary>
        public enum MessageBoxResult
        {
            Ok,
            Cancel,
            Yes,
            No
        }

        /// <summary>
        /// Initializes a new instance of the MessageBox class.
        /// </summary>
        public MessageBox()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Adds a button to the message box.
        /// </summary>
        /// <param name="caption">The text to display on the button.</param>
        /// <param name="result">The result associated with the button when clicked.</param>
        /// <param name="buttonPanel">The panel where the button will be added.</param>
        /// <param name="closeMessageBox">An action to close the message box.</param>
        /// <param name="resultSetter">An action to set the result of the message box.</param>
        /// <param name="defaultButton">Specifies whether this button should be set as the default button.</param>
        static void AddButton(string caption, MessageBoxResult result, ref StackPanel buttonPanel, Action closeMessageBox, Action<MessageBoxResult> resultSetter, bool defaultButton = false)
        {
            Button btn = new() { Content = caption };
            btn.Click += (_, __) =>
            {
                resultSetter(result);
                closeMessageBox();
            };

            buttonPanel.Children.Add(btn);
            if (defaultButton)
                resultSetter(result);
        }

        /// <summary>
        /// Displays a message box with the specified text, title, and buttons, and optionally, a parent window.
        /// </summary>
        /// <param name="parent">The parent window of the message box, if any.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="buttons">The buttons to display in the message box.</param>
        /// <returns>A Task representing the asynchronous operation, containing the result of the message box.</returns>
        public static Task<MessageBoxResult> Show(Window parent, string text, string title, MessageBoxButtons buttons)
        {
            MessageBox messageBox = new()
            {
                Title = title
            };

            TextBlock? msgBoxTextBlock = messageBox.FindControl<TextBlock>("Text");
            if (msgBoxTextBlock is not null)
            {
                msgBoxTextBlock.Text = text;
            }


            StackPanel? buttonPanel = messageBox.FindControl<StackPanel>("Buttons");
            if (buttonPanel is null)
            {
                return Task.FromResult(MessageBoxResult.Cancel);
            }

            MessageBoxResult res = MessageBoxResult.Ok;

            if (buttons == MessageBoxButtons.Ok || buttons == MessageBoxButtons.OkCancel)
                AddButton(
                    caption: "Ok",
                    result: MessageBoxResult.Ok,
                    buttonPanel: ref buttonPanel,
                    closeMessageBox: messageBox.Close,
                    resultSetter: (value) => res = value,
                    defaultButton: true
                );

            if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
            {
                AddButton(
                    caption: "Yes",
                    result: MessageBoxResult.Yes,
                    buttonPanel: ref buttonPanel,
                    closeMessageBox: messageBox.Close,
                    resultSetter: (value) => res = value,
                    defaultButton: false
                );

                AddButton(
                    caption: "No",
                    result: MessageBoxResult.No,
                    buttonPanel: ref buttonPanel,
                    closeMessageBox: messageBox.Close,
                    resultSetter: (value) => res = value,
                    defaultButton: true
                );
            }

            if (buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel)
                AddButton(
                    caption: "Cancel",
                    result: MessageBoxResult.Cancel,
                    buttonPanel: ref buttonPanel,
                    closeMessageBox: messageBox.Close,
                    resultSetter: (value) => res = value,
                    defaultButton: true
                );


            TaskCompletionSource<MessageBoxResult> taskCompletionSource = new();

            messageBox.Closed += delegate { taskCompletionSource.TrySetResult(res); };

            if (parent != null)
                messageBox.ShowDialog(parent);
            else
                messageBox.Show();

            return taskCompletionSource.Task;
        }
    }
}
