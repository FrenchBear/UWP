// UWP GroupingGridView
// A simple example of grouping GridView (left half of window) and a SemanticZoom contol (right half of window)
//
// 2018-09-20   PV


using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace GroupingGridViewNS
{
    internal class ViewModel : INotifyPropertyChanged
    {
        // Private
        private readonly Page w;


        // INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(property)));


        // Constructor
        internal ViewModel(Page w)
        {
            this.w = w;

            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x21, Character = "!", Category = "Symbols" });
            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x26, Character = "&", Category = "Symbols" });
            for (int i = 0x30; i <= 0x39; i++)
                CharactersRecords.Add(new CharacterRecord { Codepoint = i, Character = new string((char)i, 1), Category = "Digits" });
            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x3f, Character = "?", Category = "Symbols" });
            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x40, Character = "@", Category = "Symbols" });
            for (int i = 0x41; i <= 0x5A; i++)
                CharactersRecords.Add(new CharacterRecord { Codepoint = i, Character = new string((char)i, 1), Category = "Uppercase Letters" });
            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x5f, Character = "_", Category = "Symbols" });
            for (int i = 0x61; i <= 0x7A; i++)
                CharactersRecords.Add(new CharacterRecord { Codepoint = i, Character = new string((char)i, 1), Category = "Lowercase Letters" });
            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x7b, Character = "{", Category = "Symbols" });
            foreach (var c in "éèçàâêûù")
                CharactersRecords.Add(new CharacterRecord { Codepoint = (int)c, Character = new string(c, 1), Category = "Lowercase Accented Letters" });
            foreach (var c in "ÉÈÇÀÂÊÛÙ")
                CharactersRecords.Add(new CharacterRecord { Codepoint = (int)c, Character = new string(c, 1), Category = "Uppercase Accented Letters" });
            foreach (var c in "αβγδψπω")
                CharactersRecords.Add(new CharacterRecord { Codepoint = (int)c, Character = new string(c, 1), Category = "Greek Lowercase Letters" });
            foreach (var c in "ΑΒΓΔΨΠΩ")
                CharactersRecords.Add(new CharacterRecord { Codepoint = (int)c, Character = new string(c, 1), Category = "Greek Uppercase Letters" });
            foreach (var c in "ĳœӕﬀﬁﬆ")
                CharactersRecords.Add(new CharacterRecord { Codepoint = (int)c, Character = new string(c, 1), Category = "Lowercase Ligatures" });


            //var result = from cr in CharactersRecords group cr by cr.Category into grp orderby grp.Key select grp;
            CharactersRecordsCVS.Source = CharactersRecords.GroupBy(cr => cr.Category).OrderBy(g => g.Key);
            CharactersRecordsCVS.IsSourceGrouped = true;

            //foreach (var v in CharactersRecordsCVS.View.CollectionGroups)
            //{
            //    var vv = (Windows.UI.Xaml.DependencyObject)v;
            //    var zz = vv as ICollectionViewGroup;
            //    var zzg = zz.Group;
            //    var zzgt = (IGrouping<string, GroupingGridViewNS.CharacterRecord>)zzg;
            //    var xx = zzgt.Key;

            //    //Debugger.Break();
            //}
        }


        // Bindable properties
        public ObservableCollection<CharacterRecord> CharactersRecords { get; set; }  = new ObservableCollection<CharacterRecord>();

        public CollectionViewSource CharactersRecordsCVS { get; set; } = new CollectionViewSource();
    }



    public sealed partial class MainPage : Page
    {
        ViewModel vm;

        public MainPage()
        {
            this.InitializeComponent();
            vm = new ViewModel(this);
            this.DataContext = vm;
        }
    }

}
