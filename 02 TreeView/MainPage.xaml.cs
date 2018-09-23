// TreeView example
// Learning UWP
//
// 2018-09-22   PV      Almost useless in its current form (Windows 10 1803): can't bind, can't really retemplate, can't select/unselect nodes using code, selectionchanged event missing and simulating with other events is unreliable...
//                      The feeling is that 6 years after Windows 8 was released, there is still not a decent simple TreeView in modern Windows UI controls...


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
