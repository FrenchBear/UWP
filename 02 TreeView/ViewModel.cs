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
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;


namespace TreeViewNS
{
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


        private string _BlockNameFilter;
        public string BlockNameFilter
        {
            get { return _BlockNameFilter; }
            set
            {
                if (_BlockNameFilter != value)
                {
                    _BlockNameFilter = value;
                    NotifyPropertyChanged(nameof(BlockNameFilter));
                    FilterBlockTree();
                }
            }
        }

        private void FilterBlockTree()
        {
            FilterBlock(BlocksRoot);

            bool f(BlockNode bn) => bn.Name.IndexOf(BlockNameFilter, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;

            bool FilterBlock(BlockNode n)
            {
                if (n.Level == 0)
                    return f(n);
                else
                {
                    bool exp = false;
                    foreach (BlockNode child in n.Children)
                        exp |= FilterBlock(child);
                    exp |= f(n);
                    n.IsExpanded = exp;
                    return exp;
                }
            }
        }



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
}
