// Options - Learning UWP
// Data class holding values of options, resposible for saving and loading them from local storage
// Note that access to settings.Values dictionary is not async contrary to basic file operations
//
// 2018-09-26   PV


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OptionsNS
{
    class OptionBag : INotifyPropertyChanged
    {
        // Private variables
        readonly Windows.Storage.ApplicationDataContainer settings;


        // INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
          => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        // Constructor
        public OptionBag()
        {
            // Use LocalSettings to memoriqe settings (but could also use Roamingettings)
            settings = Windows.Storage.ApplicationData.Current.LocalSettings;
        }


        // Helpers to store and retrieve option value in settings
        private T GetOption<T>(string optionName, T defaultValue)
        {
            if (settings.Values.ContainsKey(optionName))
            {
                // For nullable types, the string <NULL> is replaced by actual null value (actually default(T))
                if (Nullable.GetUnderlyingType(typeof(T)) != null && settings.Values[optionName].ToString() == "<NULL>")
                    return default(T);

                return (T)settings.Values[optionName];
            }

            return defaultValue;
        }

        private void SetOption<T>(string optionName, T value)
        {
            // When storing a null in Settings.Values, it deletes the element from dictionary so later when loading back,
            // it's not possible to differentiate null value from default, which is typically not ok for a 3-state checkbox.
            // So for nullable types, we store string <NULL> in this case.
            if (Nullable.GetUnderlyingType(typeof(T)) != null && value.Equals(null))
            {
                Debug.WriteLine($"SetOption: {optionName}=<NULL>");
                settings.Values[optionName] = "<NULL>";
                return;
            }
            Debug.WriteLine($"SetOption: {optionName}={value}");
            settings.Values[optionName] = value;
        }




        // Bindable options
        public bool? ACheckBox
        {
            get { return GetOption<bool?>("ACheckBox", false); }
            set
            {
                if (ACheckBox.HasValue != value.HasValue || (ACheckBox.HasValue && ACheckBox.Value != value.Value))
                {
                    SetOption("ACheckBox", value);
                    NotifyPropertyChanged(nameof(ACheckBox));
                }
            }
        }

        public bool AToggleSwitch
        {
            get { return GetOption<bool>("AToggleSwitch", false); }
            set
            {
                if (AToggleSwitch != value)
                {
                    SetOption("AToggleSwitch", value);
                    NotifyPropertyChanged(nameof(AToggleSwitch));
                }
            }
        }

        public bool AToggleButton
        {
            get { return GetOption<bool>("AToggleButton", false); }
            set
            {
                if (AToggleButton != value)
                {
                    SetOption("AToggleButton", value);
                    NotifyPropertyChanged(nameof(AToggleButton));
                }
            }
        }

        public string CaseOption
        {
            get { return GetOption<string>("CaseOption", "None"); }
            set
            {
                if (CaseOption != value)
                {
                    SetOption("CaseOption", value);
                    NotifyPropertyChanged(nameof(CaseOption));
                }
            }
        }

        public int SliderOption
        {
            get { return GetOption<int>("SliderOption", 0); }
            set
            {
                if (SliderOption != value)
                {
                    SetOption("SliderOption", value);
                    NotifyPropertyChanged(nameof(SliderOption));
                }
            }
        }

        public string TextOption
        {
            get { return GetOption<string>("TextOption", ""); }
            set
            {
                if (TextOption != value)
                {
                    SetOption("TextOption", value);
                    NotifyPropertyChanged(nameof(TextOption));
                }
            }
        }

        public string ListOption
        {
            get { return GetOption<string>("ListOption", ""); }
            set
            {
                if (ListOption != value)
                {
                    SetOption("ListOption", value);
                    NotifyPropertyChanged(nameof(ListOption));
                }
            }
        }

    }
}
