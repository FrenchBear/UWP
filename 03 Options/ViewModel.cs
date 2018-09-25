﻿using RelayCommandNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;

namespace OptionsNS
{
    class ViewModel : INotifyPropertyChanged
    {
        // Private variables
        MainPage page;


        // INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
          => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        // Commands public interface
        public ICommand ActionCommand { get; private set; }

        public ViewModel(MainPage p)
        {
            page = p;

            // Binding commands with behavior
            ActionCommand = new AwaitableRelayCommand<object>(ActionExecute, CanAction);
        }

        private bool CanAction(object obj)
        {
            return true;
        }

        private async Task ActionExecute(object obj)
        {
            var dialog = new MessageDialog("Action text", "Action");
            await dialog.ShowAsync();
        }
    }
}