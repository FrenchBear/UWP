// Options MainPage
// Learning UWP - Options panel and local storage
//
// 2018-09-25   PV


using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;



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

            Loaded += (s, e) => QuestionButton.Focus(FocusState.Programmatic);
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"AppBarButton_Click: {sender.ToString()} {e.ToString()}");
            if (sender == AboutButton)
            {
                //Debug.WriteLine("About");
                await MessageBox.Show("Learning UWP application, focusing on options, local settings storage and command bar", "03 Options");
            }
        }

    }
}
