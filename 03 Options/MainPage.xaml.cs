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
using Windows.Storage;
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
        OptionBag opt;

        public MainPage()
        {
            this.InitializeComponent();
            vm = new ViewModel(this);
            DataContext = vm;
            opt = new OptionBag();
            OptionsPanel.DataContext = opt;


            Debug.WriteLine($"ACheckBox={opt.ACheckBox}");
            Debug.WriteLine($"AToggleSwitch={opt.AToggleSwitch}");
            Debug.WriteLine($"AToggleButton={opt.AToggleButton}");
            Debug.WriteLine($"CaseOption={opt.CaseOption}");
            Debug.WriteLine($"SliderOption={opt.SliderOption}");
            Debug.WriteLine($"TextOption={opt.TextOption}");
            Debug.WriteLine($"ListOption={opt.ListOption}");

            //opt.ACheckBox = null;
            //Debug.WriteLine($"After setting to null: ACheckBox={opt.ACheckBox}");

        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"AppBarButton_Click: {sender.ToString()} {e.ToString()}");
        }

    }
}
