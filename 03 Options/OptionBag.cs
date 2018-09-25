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
                if (Nullable.GetUnderlyingType(typeof(T)) != null && settings.Values[optionName].ToString() == "<NULL>")
                {
                    var res = default(T);
                    return res;
                }
                else
                    return (T)settings.Values[optionName];
            }
            else
                return defaultValue;
        }

        private void SetOption<T>(string optionName, T value)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) != null && value == null)
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
            get {return GetOption<bool?>("ACheckBox", false); }
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
