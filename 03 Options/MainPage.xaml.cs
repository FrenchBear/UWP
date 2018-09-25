// Options MainPage
// Learning UWP - Options panel and local storage
//
// 2018-09-25   PV


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



namespace OptionsNS
{
    public sealed partial class MainPage : Page
    {
        ViewModel vm;

        public MainPage()
        {
            this.InitializeComponent();
            vm = new ViewModel(this);
            DataContext = vm;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"AppBarButton_Click: {sender.ToString()} {e.ToString()}");
        }
    }
}
