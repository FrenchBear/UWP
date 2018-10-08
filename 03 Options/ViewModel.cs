// Options ViewModel
// Learning UWP
// Standard ViewModel implementation
//
// 2018-09-26   PV


using RelayCommandNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace OptionsNS
{
    class ViewModel : INotifyPropertyChanged
    {
        // Private variables
        readonly MainPage page;


        // INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
          => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        // Commands public interface
        public ICommand ActionCommand { get; private set; }


        // Constructor
        public ViewModel(MainPage p)
        {
            page = p;

            // Binding commands with behavior
            ActionCommand = new AwaitableRelayCommand<object>(ActionExecute, CanAction);
        }


        // Commands implementation
        private bool CanAction(object obj)
        {
            return true;
        }

        private async Task ActionExecute(object obj)
        {
            var dialog = new MessageDialog("Action text", "Action");
            await dialog.ShowAsync();
        }


#pragma warning disable RECS0154 // Parameter is never used


        // Event handler in ViewModel, allowed by x:Bind
        internal async void QuestionButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var button = await MessageBox.Show("Would you like to greet the world with a \"Hello, world\"?", "A question for you:", MessageBoxButton.YesNoCancel);
            Debug.WriteLine($"After MsgBox: button={button}");
        }


        internal async void CopyImageButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var control = page.CharImage;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(control, (int)control.Width, (int)control.Height);
            page.MyImage.Source = renderTargetBitmap;

            // Get the pixels BGRA8-format.
            // IBuffer represents a referenced array of bytes used by byte stream read and write interfaces.
            IBuffer pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
            int width = renderTargetBitmap.PixelWidth;
            int height = renderTargetBitmap.PixelHeight;
            // The GetPixelsAsync() return value enables passing the result to a WriteableBitmap and its PixelBuffer.
            // Another alternative is passing the buffer to a BitmapEncoder. If you want an array of bytes, use a
            // DataReader and the FromBuffer method to help with the conversion.

            /*
            // ToArray() gets a copy of the buffer
            // Updating the array doesn't update the buffer
            var a = pixelBuffer.ToArray();
            var bb0 = pixelBuffer.GetByte(0);
            var ba0 = a[0];
            a[0] = 73;
            var ab0 = pixelBuffer.GetByte(0);
            var aa0 = a[0];


            // pixelBuffer.AsStream() provides a Stream view of the IBuffer:
            // Updating the stream actually updates the buffer
            var str = pixelBuffer.AsStream();
            var b1 = str.CanRead;
            var b2 = str.CanWrite;
            str.Seek(0, SeekOrigin.Begin);
            str.WriteByte(73);
            str.Flush();

            var a2 = pixelBuffer.ToArray();
            var c0 = a2[0];
            */

            await CopyImageUsingMemoryStreamAndSoftwareBitmap(pixelBuffer, width, height);
            //await CopyImageUsingMemoryStream(pixelBuffer, width, height);
            //await CopyImageUsingStorageFile(pixelBuffer, width, height);

        }

        internal async static Task CreateLowRawDataFile(IBuffer pixelBuffer, int width, int height)
        {
            // Low-level bytes file to validate content
            string filename = $"pic.{width}x{height}.dat";
            StorageFolder sfo = await StorageFolder.GetFolderFromPathAsync(@"C:\temp");
            StorageFile sf = await sfo.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(sf, pixelBuffer);
        }


        static InMemoryRandomAccessStream ma1;

        // Variant of CopyImageUsingMemoryStream avoiding use of pixelBuffer.ToArray(), but
        // new SoftwareBitmap() and SoftwareBitmap.CopyFromBuffer have the save duplication cost
        internal async static Task CopyImageUsingMemoryStreamAndSoftwareBitmap(IBuffer pixelBuffer, int width, int height)
        {
            SoftwareBitmap sbmp = new SoftwareBitmap(BitmapPixelFormat.Bgra8, width, height, BitmapAlphaMode.Straight);
            sbmp.CopyFromBuffer(pixelBuffer);

            ma1 = new InMemoryRandomAccessStream();
            BitmapEncoder be = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ma1);
            be.SetSoftwareBitmap(sbmp);
            await be.FlushAsync();
            var rasr = RandomAccessStreamReference.CreateFromStream(ma1);
            ClipboardSetImage(rasr);
        }


        static InMemoryRandomAccessStream ma2;

        // Copy image using a stream provided by InMemoryRandomAccessStream
        // Key point: declare ma at class level to prevent GC destruction
        internal async static Task CopyImageUsingMemoryStream(IBuffer pixelBuffer, int width, int height)
        {
            ma2 = new InMemoryRandomAccessStream();
            BitmapEncoder be = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ma2);
            be.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)width, (uint)height, 96, 96, pixelBuffer.ToArray());
            await be.FlushAsync();
            var rasr = RandomAccessStreamReference.CreateFromStream(ma2);
            ClipboardSetImage(rasr);
        }


        // ToDo: Use a temporary file
        internal async static Task CopyImageUsingStorageFile(IBuffer pixelBuffer, int width, int height)
        {
            StorageFolder sfo = await StorageFolder.GetFolderFromPathAsync(@"C:\temp");
            StorageFile pngFile = await sfo.CreateFileAsync("boar.png", CreationCollisionOption.ReplaceExisting);
            IRandomAccessStream pngStream = await pngFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            BitmapEncoder be = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, pngStream);
            be.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)width, (uint)height, 96, 96, pixelBuffer.ToArray());
            await be.FlushAsync();

            var rasr = RandomAccessStreamReference.CreateFromStream(pngStream);
            ClipboardSetImage(rasr);

            // Dispose makes clipboard content lost
            //pngStream.Dispose();
        }


        // Copy pixelBuffer to a WriteableBitmap, but at the end we just get anoher ImageSource and its IBuffer PixelBuffer,
        // so basically back to square one
        internal static void CreateWriteableBitmap(IBuffer pixelBuffer, int width, int height)
        {
            WriteableBitmap bitmap = new WriteableBitmap(width, height);
            using (Stream stream = bitmap.PixelBuffer.AsStream())
            {
                stream.Write(pixelBuffer.ToArray(), 0, width * height * 4);
            }
            //page.MyImage.Source = bitmap;
        }



        internal static void ClipboardSetImage(RandomAccessStreamReference bmp)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetBitmap(bmp);
            try
            {
                Clipboard.SetContent(dataPackage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error copying text into clipboard: " + ex.Message);
            }

        }


    }
}
