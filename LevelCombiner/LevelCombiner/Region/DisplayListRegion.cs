using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelCombiner
{
    class DisplayListRegion : Region
    {
        public bool DLFixesNeeded;
        public bool isFogEnabled;
        public bool isEnvcolorEnabled;
        public int FCCount = 0;
        public int FCCountFixed = 0;
        public Int64 B9cmdfirst = 0;
        public int layer = 4; 

        public DisplayListRegion(int start, int length, bool isFogEnabled, bool isEnvcolorEnabled, int FCcount, Int64 B9cmd, int layer) : base(start, length, RegionState.DisplayList)
        {
            DLFixesNeeded = true;
            this.isFogEnabled =      isFogEnabled;
            this.isEnvcolorEnabled = isEnvcolorEnabled;
            this.FCCount = FCcount;
            this.B9cmdfirst = B9cmd;
            this.layer = layer;
        }

        public DisplayListRegion(string dirname, int area = -1, int model = -1, int number = -1) : base(dirname, RegionState.DisplayList, area, model, number)
        {
            DLFixesNeeded = false;
        }

        public override void Relocate(RelocationTable table)
        {
            DisplayList.PerformRegionRelocation(this, table);
        }
    }
}
