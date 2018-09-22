// UniBlocks
// Hierarchical example data for TreeView example
// 2018-09-22   PV

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace TreeView
{
    public class BlockRecord
    {
        public int Begin { get; private set; }
        public int End { get; private set; }
        public string BlockName { get; private set; }
        public string Level1Name { get; private set; }
        public string Level2Name { get; private set; }
        public string Level3Name { get; private set; }

        // Sorting order matching hierarchy
        public int Rank { get; internal set; }

        public string BlockNameAndRange => $"{BlockName} {Begin:X4}..{End:X4}";


        internal BlockRecord(int Begin, int End, string BlockName, string Level1Name, string Level2Name, string Level3Name)
        {
            this.Begin = Begin;
            this.End = End;
            this.BlockName = BlockName;
            this.Level1Name = Level1Name;
            this.Level2Name = Level2Name;
            this.Level3Name = Level3Name;
        }

        public override string ToString()
        {
            return $"BlockRecord(Range={Begin:X4}..{End:X4}, Block={BlockName}, L1={Level1Name}, L2={Level2Name}, L3={Level3Name})";
        }
    }


    static class UniBlocks
    {
        // Real internal dictionaries used to store Unicode data
        private static readonly Dictionary<int, BlockRecord> block_map = new Dictionary<int, BlockRecord>();

        public static ReadOnlyDictionary<int, BlockRecord> BlockRecords => new ReadOnlyDictionary<int, BlockRecord>(block_map);

        static UniBlocks()
        {
            // Read blocks
            using (var sr = new StreamReader(GetResourceStream("MetaBlocks.txt")))
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.Length == 0 || line[0] == '#') continue;
                    string[] fields = line.Split(';');
                    string[] field0 = fields[0].Replace("..", ";").Split(';');
                    int begin = int.Parse(field0[0], NumberStyles.HexNumber);
                    int end = int.Parse(field0[1], NumberStyles.HexNumber);
                    BlockRecord br = new BlockRecord(begin, end, fields[1], fields[2], fields[3], fields[4]);
                    block_map.Add(begin, br);
                }

            // Compute rank using an integer of format 33221100 where 33 is index of L3 block, 22 index of L2 block in L3...
            int rank3 = 0;
            foreach (var l3 in block_map.Values.GroupBy(b => b.Level3Name).OrderBy(g => g.Key))
            {
                int rank2 = 0;
                foreach (var l2 in l3.GroupBy(b => b.Level2Name))
                {
                    int rank1 = 0;
                    foreach (var l1 in l2.GroupBy(b => b.Level1Name))
                    {
                        int rank0 = 0;
                        foreach (var l0 in l1)
                        {
                            l0.Rank = rank0 + 100 * (rank1 + 100 * (rank2 + 100 * rank3));
                            rank0++;
                        }
                        rank1++;
                    }
                    rank2++;
                }
                rank3++;
            }

        }


        // Returns stream from embedded resource name
        private static Stream GetResourceStream(string name)
        {
            name = "." + name;
            var assembly = typeof(UniBlocks).GetTypeInfo().Assembly;
            var qualifiedName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (qualifiedName == null)
                return null;
            else
                return assembly.GetManifestResourceStream(qualifiedName);
        }

    }
}
