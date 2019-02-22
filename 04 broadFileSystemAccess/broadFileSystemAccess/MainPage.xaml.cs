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
// DO NOT FORGET in Windows Settings>Privacy>File system to grant broadFileSystemAccess to the app!
//
// App need to target/minimum WIndows 10 1803
//
// Useful information:
// https://readdy.net/Notes/Details/440?ds=m
// https://blogs.windows.com/buildingapps/2018/05/18/console-uwp-applications-and-file-system-access/


using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace broadFileSystemAccess
{
    public  sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            //Loaded += MainPage_Loaded;
        }

        private async void BtnCopy_Click(object sender, RoutedEventArgs e)

        {

            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(tbxFolder.Text);
            StorageFile file = await folder.GetFileAsync(tbxFile.Text);
            StorageFile copyFile = await file.CopyAsync(folder, "Copied_File.txt", NameCollisionOption.ReplaceExisting);

        }

        //private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        //{
        //    Debug.WriteLine("Page loaded");
        //    var localfolder = ApplicationData.Current.LocalFolder.Path;
        //    var array = localfolder.Split('\\');
        //    var username = array[2];
        //    string _cloudPath = @"C:\temp\coro.py";

        //    string _dbLocalFolderPath = ApplicationData.Current.LocalFolder.Path;
        //    const string _dbLocalFileName = "local_coro.py";

        //    await CopyFileAsync(_cloudPath, _dbLocalFolderPath, _dbLocalFileName);
        //    Debug.WriteLine("File copied");
        //}


        private static async Task CopyFileAsync(string sourceFilePath,
            string destinationFilePath, string destinationFileName,
            NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            var sourceFile = await StorageFile.GetFileFromPathAsync(sourceFilePath);
            var destinationFolder = await StorageFolder.GetFolderFromPathAsync(destinationFilePath);
            await sourceFile.CopyAsync(destinationFolder, destinationFileName, option);
        }
    }
}
