// MessageBox
// Learning UWP
// https://stackoverflow.com/questions/22909329/universal-apps-messagebox-the-name-messagebox-does-not-exist-in-the-current
//
// Note: ContentDialog is obsolete for WIndows 10, use ContentDialog
//
// ToDo: Implement a traditional MsgBox interface with flags for buttons and icon
//
// 2018-09-26   PV


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;


namespace OptionsNS
{
    public static class MsgBox
    {
        [Obsolete("Only for Windows 8")]
        static public async Task<int> ShowMessageDialog(string mytext)
        {
            MessageDialog msgbox = new MessageDialog(mytext);
            // msgbox.Title = "Title";

            msgbox.Commands.Clear();
            msgbox.Commands.Add(new UICommand { Label = "Yes", Id = 1 });
            msgbox.Commands.Add(new UICommand { Label = "No", Id = 2 });
            msgbox.Commands.Add(new UICommand { Label = "Cancel", Id = 0 });

            // To Trigger some Function When "Yes" or "No" is clicked, You can also use:
            // msgbox.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.TriggerThisFunctionForYes)));
            // msgbox.Commands.Add(new UICommand("No", new UICommandInvokedHandler(this.TriggerThisFunctionForNo)));

            var res = await msgbox.ShowAsync();
            return (int)res.Id;
        }


        static public async Task<int> ShowContentDialog(string mytext, string title = "")
        {
            ContentDialog msgbox = new ContentDialog
            {
                Content = mytext,
                Title = title,

                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult res = await msgbox.ShowAsync();
            return (int)res;
        }

    }
}
