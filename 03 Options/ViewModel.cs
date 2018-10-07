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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
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


            // Make a DataReader of IBuffer
            //DataReader dr = DataReader.FromBuffer(pixelBuffer);

            StorageFile sf = await StorageFile.GetFileFromPathAsync(@"C:\temp\data.b");
            await FileIO.WriteBufferAsync(sf, pixelBuffer);




            /*
            using (var fs = new FileStream(@"C:\temp\data.b", FileMode.Create))
            using (DataWriter dw = new DataWriter( sf))
            {

            }
            */



            //using (var fs = new FileStream(@"C:\temp\data.b", FileMode.Create))
            //using (var bw = new BinaryWriter(fs))
            //    bw.Write(pixelBuffer);

            /*
            // Make a MemoryBuffer from IBuffer
            MemoryBuffer mb = Windows.Storage.Streams.Buffer.CreateMemoryBufferOverIBuffer(pixelBuffer);

            InMemoryRandomAccessStream ma = new InMemoryRandomAccessStream();
                IOutputStream os = ma.GetOutputStreamAt(0);
                DataWriter dw = new DataWriter(os);
                dw.WriteBuffer(pixelBuffer);
                await dw.StoreAsync();
                await dw.FlushAsync();

                //await ma.WriteAsync(pixelBuffer);
                ////var inputStream = ma.GetInputStreamAt(0);
                ////var mairas = ma as IRandomAccessStream;
                //await ma.FlushAsync();

                var rasr = RandomAccessStreamReference.CreateFromStream(ma);
                ClipboardSetImage(rasr);
            */
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
