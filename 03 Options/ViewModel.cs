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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Input;


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



        // Event handler in ViewModel, allowed by x:Bind
#pragma warning disable RECS0154 // Parameter is never used
        internal async void Button_Tapped(object sender, TappedRoutedEventArgs e)
#pragma warning restore RECS0154 // Parameter is never used
        {
            int button = await MsgBox.ShowContentDialog("Would you like to greet the world with a \"Hello, world\"?", "A question for you:");
            Debug.WriteLine($"After MsgBox: button={button}");
        }

    }
}
