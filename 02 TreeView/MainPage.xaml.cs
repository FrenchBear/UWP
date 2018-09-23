// TreeView example
// Learning UWP
//
// 2018-09-22   PV      Almost useless in its current form (Windows 10 1803): can't bind, can't really retemplate, can't select/unselect nodes using code, selectionchanged event missing and simulating with other events is unreliable...
//                      The feeling is that 6 years after Windows 8 was released, there is still not a simple TreeView in modern Windows UI controls...


using RelayCommandNS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace TreeViewNS
{
    public sealed partial class MainPage : Page
    {
        private ViewModel vm;

        public MainPage()
        {
            this.InitializeComponent();
            vm = new ViewModel(this);
            DataContext = vm;

            // Use ForEach() to save 1 line!
            vm.BlocksRoot.Children.ForEach(n => BlocksTreeView.RootNodes.Add(n));
        }


        // Simulate "SelectionChanged" missing event with mouse and key events
        // But this is unreliable, doubletap event gets fired before SelectedNodes has been refreshed...
        private void BlocksTreeView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("BlocksTreeView_Tapped");
            ShowSelectedCount();
        }
        private void BlocksTreeView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Debug.WriteLine("BlocksTreeView_DoubleTapped");
            ShowSelectedCount();
        }

        private void BlocksTreeView_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Space)
                ShowSelectedCount();
        }


        private void ShowSelectedCount()
        {
            vm.SelectedBlocksCount = BlocksTreeView.SelectedNodes.Count;
            vm.SelectedBlocks.Clear();
            //var SelectedBlocksSet = new HashSet<BlockRecord>();
            foreach (var item in BlocksTreeView.SelectedNodes)
                if ((item as BlockNode).Level == 0)
                    vm.SelectedBlocks.Add((item as BlockNode).Block);
        }

        private void ShowHideButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            BlocksTreeView.SelectAll();
            ShowSelectedCount();
        }

        private void UnselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            BlocksTreeView.SelectedNodes.Clear();
        }

    }



    internal class ViewModel : INotifyPropertyChanged
    {
        private readonly MainPage w;


        // INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        // Commands public interface
        public ICommand ShowLevelCommand { get; private set; }


        public ViewModel(MainPage w)
        {
            this.w = w;


            // Binding commands with behavior
            ShowLevelCommand = new RelayCommand<object>(ShowLevelExecute);


            // Data Initialization
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
                            var node = new BlockNode(l0.BlockName, 0, l0);
                            r1.Children.Add(node);
                        }
                    }
                }
            }
        }


        // ==============================================================================================
        // Bindable properties

        public List<BlockRecord> BlocksRecordsList { get; set; }


        public BlockNode BlocksRoot { get; set; }


        private int _SelectedBlocksCount;
        public int SelectedBlocksCount
        {
            get => _SelectedBlocksCount;
            set
            {
                if (_SelectedBlocksCount != value)
                {
                    _SelectedBlocksCount = value;
                    NotifyPropertyChanged(nameof(SelectedBlocksCount));
                }
            }
        }

        public ObservableCollection<BlockRecord> SelectedBlocks { get; set; } = new ObservableCollection<BlockRecord>();


        // ==============================================================================================
        // Commands

        // Helper performing a given action on a node and all its decendants
        void ActionAllNodes(BlockNode n, Action<BlockNode> a)
        {
            a(n);
            foreach (BlockNode child in n.Children)
                ActionAllNodes(child, a);
        }

        private void ShowLevelExecute(object param)
        {
            int level = int.Parse(param as string);
            ActionAllNodes(BlocksRoot, n => { n.IsExpanded = (n.Level != level); });
        }
    }


    internal class BlockNode : TreeViewNode
    {
        public BlockNode(string name, int level, BlockRecord block = null)
        {
            Name = name;
            Level = level;
            Block = block;
            //Content = $"{level}: {name}";
            Content = name;
            IsExpanded = true;
        }

        public int Level { get; set; }
        public string Name { get; set; }
        public BlockRecord Block { get; set; }
    }



    static class ExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }


    public class BlockGroupItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BlockTemplate { get; set; }
        public DataTemplate GroupL1Template { get; set; }
        public DataTemplate GroupTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var blockItem = (BlockNode)item;
            if (blockItem.Level == 0) return BlockTemplate;
            if (blockItem.Level == 1) return GroupL1Template;
            return GroupTemplate;
        }
    }
}
