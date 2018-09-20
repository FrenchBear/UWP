// GroupingGridView
// A simple example of grouping GridView.
// Took me hours until I understand there is no way to do without a Xaml CollectionViewSource resource
// and bind the gridview to it using StaticResource.  Any attempt to create CollectionViewSource in CS code
// and do exactly the same fails with an error "System.ArgumentException: 'Value does not fall within the expected range.'"
//
// 2018-09-20   PV


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    public sealed partial class MainPage : Page
    {
        ObservableCollection<CharacterRecord> CharactersRecords = new ObservableCollection<CharacterRecord>();

        public MainPage()
        {
            this.InitializeComponent();

            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x21, Character = "!", Category = "Symbols" });
            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x26, Character = "&", Category = "Symbols" });
            for (int i = 0x30; i <= 0x39; i++)
                CharactersRecords.Add(new CharacterRecord { Codepoint = i, Character = new string((char)i, 1), Category = "Digits" });
            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x3f, Character = "?", Category = "Symbols" });
            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x40, Character = "@", Category = "Symbols" });
            for (int i = 0x41; i <= 0x5A; i++)
                CharactersRecords.Add(new CharacterRecord { Codepoint = i, Character = new string((char)i, 1), Category = "Letters" });
            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x5f, Character = "_", Category = "Symbols" });
            CharactersRecords.Add(new CharacterRecord { Codepoint = 0x7b, Character = "{", Category = "Symbols" });


            //CharactersGrivView.ItemsSource = CharactersRecords;

            /*
            var g = CharactersRecords.GroupBy(cr => cr.Category);
            var cs = new CollectionViewSource();
            cs.Source = g;
            cs.IsSourceGrouped = true;
            CharactersGrivView.ItemsSource = cs;
            */

            
            //var result = from cr in CharactersRecords group cr by cr.Category into grp orderby grp.Key select grp;
            //groupInfoCVS.Source = result;

            /*
            var result = from cr in CharactersRecords group cr by cr.Category into grp orderby grp.Key select grp;
            var cs = new CollectionViewSource();
            cs.Source = result;
            cs.IsSourceGrouped = true;
            lbGroupInfoCVS.ItemsSource = cs;
            */

            GroupedCharsSource.Source = CharactersRecords.GroupBy(cr => cr.Category).OrderBy(k => k.Key);
        }
    }

}
