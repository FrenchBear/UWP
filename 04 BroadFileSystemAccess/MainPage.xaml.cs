// broadFileSystemAccess Example
// Learning UWP
//
// Demo of this extended feature
//
// In Package.appxmanifest:
// <Package ...
//  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
//  IgnorableNamespaces="... rescap">
// 
// <Capabilities>
//   ...
//   <rescap:Capability Name="broadFileSystemAccess" />
// </Capabilities>
//
// DO NOT FORGET
// - in Windows Settings>Privacy>File system to grant broadFileSystemAccess to the app!
// - in Windows Settings>Apps>BroadFileSystemAccess>Advanced Options>App permissions to switch File system to "On"
//
// App need to target/minimum Windows 10 1803
//
// Useful information:
// https://readdy.net/Notes/Details/440?ds=m
// https://blogs.windows.com/buildingapps/2018/05/18/console-uwp-applications-and-file-system-access/

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#pragma warning disable IDE0051 // Remove unused private members

namespace broadFileSystemAccess;

public sealed partial class MainPage: Page
{
    public MainPage() => InitializeComponent();//Loaded += MainPage_Loaded;

    private async void BtnCopy_Click(object sender, RoutedEventArgs e)

    {

        StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(tbxFolder.Text);
        string filefp = Path.Combine(tbxFolder.Text, tbxFile.Text);
        if (!File.Exists(filefp))
        {
            var dialog = new MessageDialog("Can't find file " + filefp, "BroadFileSystemAccess");
            await dialog.ShowAsync();
            return;
        }
        StorageFile file = await folder.GetFileAsync(tbxFile.Text);
        StorageFile _ = await file.CopyAsync(folder, "Copied_File.txt", NameCollisionOption.ReplaceExisting);

    }

    private static async Task CopyFileAsync(string sourceFilePath,
        string destinationFilePath, string destinationFileName,
        NameCollisionOption option = NameCollisionOption.ReplaceExisting)
    {
        var sourceFile = await StorageFile.GetFileFromPathAsync(sourceFilePath);
        var destinationFolder = await StorageFolder.GetFolderFromPathAsync(destinationFilePath);
        await sourceFile.CopyAsync(destinationFolder, destinationFileName, option);
    }
}
