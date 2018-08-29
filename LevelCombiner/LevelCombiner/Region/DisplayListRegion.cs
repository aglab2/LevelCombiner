using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelCombiner
{
    class DisplayListRegion : Region
    {
        public DisplayListRegion(int start, int length) : base(start, length, RegionState.DisplayList) { }

        public DisplayListRegion(string dirname, int area = -1, int model = -1, int number = -1) : base(dirname, RegionState.DisplayList, area, model, number) { }

        public override void Relocate(RelocationTable table)
        {
            DisplayList.PerformRegionRelocation(this, table);
        }
    }
}
