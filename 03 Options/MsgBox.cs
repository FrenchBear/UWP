// MessageBox
// Learning UWP
// https://stackoverflow.com/questions/22909329/universal-apps-messagebox-the-name-messagebox-does-not-exist-in-the-current
//
// Note: ContentDialog is obsolete for WIndows 10, use ContentDialog
//
// ToDo: Implement icon support
//
// 2018-09-26   PV


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace OptionsNS
{
    public static class MessageBox
    {
        /// <summary>
        /// Displays a message box that has a message, title bar caption, button, and icon;
        /// and that accepts a default message box result, and returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A MessageBoxButton value that specifies which button or buttons to display.</param>
        /// <param name="icon">A MessageBoxImage value that specifies the icon to display.</param>
        /// <param name="defaultResult">A MessageBoxResult value that specifies the default result of the message box.</param>
        /// <returns>A MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        static public async Task<MessageBoxResult> Show(string messageBoxText, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            ContentDialog msgbox = new ContentDialog
            {
                Content = messageBoxText,
                Title = caption,
                Background = (Brush)Application.Current.Resources["SystemControlPageBackgroundChromeLowBrush"]
            };

            MessageBoxResult[] tres = new MessageBoxResult[3];
            tres[0] = MessageBoxResult.Cancel;

            switch (button)
            {
                case MessageBoxButton.OK:
                    msgbox.PrimaryButtonText = "OK";
                    tres[1] = MessageBoxResult.OK;
                    break;

                case MessageBoxButton.OKCancel:
                    msgbox.PrimaryButtonText = "OK";
                    msgbox.CloseButtonText = "Cancel";
                    tres[1] = MessageBoxResult.OK;
                    break;

                case MessageBoxButton.YesNo:
                    msgbox.PrimaryButtonText = "Yes";
                    msgbox.SecondaryButtonText = "No";
                    tres[1] = MessageBoxResult.Yes;
                    tres[2] = MessageBoxResult.No;
                    break;

                case MessageBoxButton.YesNoCancel:
                    msgbox.PrimaryButtonText = "Yes";
                    msgbox.SecondaryButtonText = "No";
                    msgbox.CloseButtonText = "Cancel";
                    tres[1] = MessageBoxResult.Yes;
                    tres[2] = MessageBoxResult.No;
                    break;
            }


            ContentDialogResult res = await msgbox.ShowAsync();
            return tres[(int)res];
        }

    }

    /// <summary>
    /// Specifies the buttons that are displayed on a message box. Used as an argument of the MsgBox.Show method.
    /// </summary>
    public enum MessageBoxButton
    {
        //     The message box displays an OK button.
        OK = 0,
        //     The message box displays OK and Cancel buttons.
        OKCancel = 1,
        //     The message box displays Yes, No, and Cancel buttons.
        YesNoCancel = 3,
        //     The message box displays Yes and No buttons.
        YesNo = 4
    }


    /// <summary>
    /// Specifies the icon that is displayed by a message box.
    /// </summary>
    public enum MessageBoxImage
    {
        //     No icon is displayed.
        None = 0,
        //     The message box contains a symbol consisting of a white X in a circle with a
        //     red background.
        Hand = 16,
        //     The message box contains a symbol consisting of white X in a circle with a red
        //     background.
        Stop = 16,
        //     The message box contains a symbol consisting of white X in a circle with a red
        //     background.
        Error = 16,
        //     The message box contains a symbol consisting of a question mark in a circle.
        Question = 32,
        //     The message box contains a symbol consisting of an exclamation point in a triangle
        //     with a yellow background.
        Exclamation = 48,
        //     The message box contains a symbol consisting of an exclamation point in a triangle
        //     with a yellow background.
        Warning = 48,
        //     The message box contains a symbol consisting of a lowercase letter i in a circle.
        Asterisk = 64,
        //     The message box contains a symbol consisting of a lowercase letter i in a circle.
        Information = 64
    }


    /// <summary>
    /// Specifies which message box button that a user clicks. MessageBoxResult is returned by the MsgBox.Show method.
    /// </summary>
    public enum MessageBoxResult
    {
        //     The message box returns no result.
        None = 0,
        //     The result value of the message box is OK.
        OK = 1,
        //     The result value of the message box is Cancel.
        Cancel = 2,
        //     The result value of the message box is Yes.
        Yes = 6,
        //     The result value of the message box is No.
        No = 7
    }
}
