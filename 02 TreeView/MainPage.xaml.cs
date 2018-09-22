// TreeView
// Learning UWP
// 2018-09-22   PV


using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace TreeView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ViewModel vm;

        public MainPage()
        {
            this.InitializeComponent();
            vm = new ViewModel(this);
            DataContext = vm;

            BlocksTreeView.RootNodes.Add(vm.BlocksRoot);
        }
    }


    internal class ViewModel : INotifyPropertyChanged
    {
        private readonly MainPage w;


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(property)));


        public ViewModel(MainPage w)
        {
            this.w = w;
            BlocksRecordsList = UniBlocks.BlockRecords.Values.OrderBy(br => br.Rank).ToList();

            BlocksRoot = new BlockNode("TopLevel", 4);
            foreach (var l3 in BlocksRecordsList.GroupBy(b => b.Level3Name).OrderBy(g => g.Key))
            {
                var r3 = new BlockNode(l3.Key, 3);
                BlocksRoot.Children.Add(r3);
                foreach (var l2 in l3.GroupBy(b => b.Level2Name))
                {
                    var r2 = new BlockNode(l2.Key, 2);
                    r3.Children.Add(r2);
                    foreach (var l1 in l2.GroupBy(b => b.Level1Name))
                    {
                        // Special case, l1 can be empty
                        TreeViewNode r1;
                        if (l1.Key.Length > 0)
                        {
                            r1 = new BlockNode(l1.Key, 1);
                            r2.Children.Add(r1);
                        }
                        else
                            r1 = r2;
                        // Blocks
                        foreach (var l0 in l1)
                        {
                            var node = new BlockNode(l0.BlockName, 0);
                            r1.Children.Add(node);
                        }
                    }
                }
            }
        }


        public List<BlockRecord> BlocksRecordsList { get; set; }

        public TreeViewNode BlocksRoot { get; set; }
    }


    internal class BlockNode : TreeViewNode
    {

        public BlockNode(string name, int level)
        {
            Name = name;
            Level = level;
            Content = $"{level}: {name}";
            IsExpanded = true;
        }

        public int Level { get; set; }
        public string Name { get; set; }
    }
}
