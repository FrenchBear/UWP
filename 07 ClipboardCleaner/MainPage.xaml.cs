// ClipboardCleaner
// Simple app to remove circled numbers from clipboard data, copied from FlientPython
//
// 2023-07-25   PV  First version
//
// Note for filesystem access:
// - Update Package.appxmanifest to add capability broadFileSystemAccess
//     https://www.jasongaylord.com/blog/2021/11/17/uwp-file-access-denied
// - In Aettings, Apps, Installed apps, ClipboardCleaner, Advanced options, App permissions, turn on File system
// - In Settings, Privacy & security, File system, allow ClipboardCleaner to access file system

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ClipboardCleaner;

public sealed partial class MainPage: Page
{
    public MainPage()
    {
        InitializeComponent();

        ClipboardContentTextBox.FontFamily = new FontFamily("Iosevka");

        //Clipboard.ContentChanged += Clipboard_ContentChanged;

        // Process initial content
        //Loaded += async (s, e) => await UpdateClipboard();
    }

    private async void Clipboard_ContentChanged(object sender, object e)
    {
        try
        {
            await UpdateClipboard();

        }
        catch (Exception ex)
        {
            var d0 = new MessageDialog("Exception: "+ex.Message);
            await d0.ShowAsync();
        }
    }

    private bool IgnoreClipboardUpdate = false;

    private async Task UpdateClipboard()
    {
        //var d0 = new MessageDialog("Step 0");
        //await d0.ShowAsync();
        //await LogTrace("0");

        if (IgnoreClipboardUpdate)
            return;

        DataPackageView dataPackageView = Clipboard.GetContent();
        if (dataPackageView.Contains(StandardDataFormats.Text))
        {
            string text = await dataPackageView.GetTextAsync();
            string cleanText = CleanNumbers(text);
            ClipboardContentTextBox.Text = cleanText;

            if (cleanText == text)
            {
                StatusLabel.Text = "Nothing to clean";
            }
            else
            {
                if (AutoCleanCheckbox.IsChecked == true)
                    SetClipboardText(cleanText);
                else
                {
                    StatusLabel.Text = "Content cleaned, clipboard not updated";
                }
            }
        }
        else
        {
            StatusLabel.Text = "Clipboard content not text";
            ClipboardContentTextBox.Text = "";
        }
    }

    private async Task ZLogTrace(string msg)
    {
        // https://learn.microsoft.com/en-us/windows/uwp/files/quickstart-reading-and-writing-files

        StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(@"C:\Temp");
        StorageFile file;
        try
        {
            file = await folder.GetFileAsync("log.txt");
        }
        catch (Exception)
        {
            file = await folder.CreateFileAsync("log.txt");
        }

        // https://stackoverflow.com/questions/34539016/append-to-a-text-file-not-overwrite
        await FileIO.AppendTextAsync(file, msg+"\n");

        // https://learn.microsoft.com/en-us/dotnet/api/system.io.windowsruntimestorageextensions.openstreamforwriteasync?view=dotnet-plat-ext-3.1
        //StorageFile file = await folder.CreateFileAsync("log.txt");
        //using (StreamWriter sw = new StreamWriter(await file.OpenStreamForWriteAsync(), Encoding.UTF8, 1024, false))
        //{
        //    await sw.WriteLineAsync(msg);
        //}
    }

    private void SetClipboardText(string text)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(text);
        try
        {
            IgnoreClipboardUpdate = true;
            Clipboard.SetContent(dataPackage);
            StatusLabel.Text = "Content cleaned, clipboard updated";
            IgnoreClipboardUpdate = false;
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error copying text into clipboard: " + ex.Message;
        }
    }

    private static readonly Regex CircledNumberRegex = new(@"[❶-❿⓫-⓴]", RegexOptions.Multiline);

    private static string CleanNumbers(string text)
    {
        var newText = CircledNumberRegex.Replace(text, string.Empty);
        if (newText!=text)
            return string.Join("\r\n", newText.Split("\r\n").Select(line => line.TrimEnd()));
        else
            return text;
    }

    private void UpdateClipboardButton_Click(object sender, RoutedEventArgs e)
        => SetClipboardText(ClipboardContentTextBox.Text);

    private async void AnalyzeClipboardButton_Click(object sender, RoutedEventArgs e)
        => await UpdateClipboard();
}

