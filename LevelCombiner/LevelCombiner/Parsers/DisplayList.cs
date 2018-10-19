using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelCombiner
{
    class DisplayList
    {
        delegate void RegionParseCmd(ROM rom, List<Region> regions, RegionParseState state);
        delegate void RelocationParseCmd(ROM rom, RelocationTable table, DisplayListRegion region);

        static RegionParseCmd[] parser = new RegionParseCmd[0xFF];
        static RelocationParseCmd[] relocationParser = new RelocationParseCmd[0xFF];

        static DisplayList()
        {
            Type t = typeof(DisplayList);
            for (int i = 0x00; i < 0xFF; i++)
            {
                parser[i] = RegionParse_common;

                string name = "RegionParse_cmd" + string.Format("{0:X2}", i);
                MethodInfo info = t.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);
                if (info == null)
                    continue;

                RegionParseCmd cmd = Delegate.CreateDelegate(typeof(RegionParseCmd), info) as RegionParseCmd;
                if (cmd == null)
                    continue;

                parser[i] = cmd;
            }

            for (int i = 0; i < 0xFF; i++)
            {
                relocationParser[i] = RelocationParse_common;

                string name = "RelocationParse_cmd" + string.Format("{0:X2}", i);
                MethodInfo info = t.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);
                if (info == null)
                    continue;

                RelocationParseCmd cmd = Delegate.CreateDelegate(typeof(RelocationParseCmd), info) as RelocationParseCmd;
                if (cmd == null)
                    continue;

                relocationParser[i] = cmd;
            }
        }
        
        class RegionParseState
        {
            public SortedRegionList textureData;
            public SortedRegionList vertexData;
            public SortedRegionList lightData;
            public Stack<int> retAddressStack;
            public int FDAddress;

            public bool isFogEnabled;
            public bool isEnvColorEnabled;
            public int FCCount;
            public Int64 B9cmdfirst;

            public RegionParseState()
            {
                textureData = new SortedRegionList();
                vertexData = new SortedRegionList();
                lightData = new SortedRegionList();
                FDAddress = -1;

                retAddressStack = new Stack<int>();
            }
        }

        public static void PerformRegionParse(ROM rom, List<Region> regions, int offset, int layer)
        {
            PerformRegionParseInternal(rom, regions, offset, layer);
        }

        static RegionParseState PerformRegionParseInternal(ROM rom, List<Region> regions, int offset, int layer)
        {
            RegionParseState state = new RegionParseState();
            rom.PushOffset(offset);
            try
            {
                int cmd = 0;
                do
                {
                    cmd = rom.Read8();
                    parser[cmd](rom, regions, state);
                    rom.AddOffset(8);
                }
                while (cmd != 0xB8);

                Region region;
                SortedRegionList graphicsData = new SortedRegionList();

                foreach (KeyValuePair<int, int> lightRegion in state.lightData.RegionList)
                {
                    //region = new Region(lightRegion.Key, lightRegion.Value, RegionState.LightData);
                    graphicsData.AddRegion(lightRegion.Key, lightRegion.Value);
                    //regions.Add(region);
                }

                // kostul
                if (state.lightData.RegionList.Count == 0)
                    graphicsData.AddRegion(rom.GetROMAddress(0x0E000000), 0x10);

                foreach (KeyValuePair<int, int> textureRegion in state.textureData.RegionList)
                {
                    //region = new Region(textureRegion.Key, textureRegion.Value, RegionState.TextureInfo);
                    graphicsData.AddRegion(textureRegion.Key, textureRegion.Value);
                    //regions.Add(region);
                }

                foreach (KeyValuePair<int, int> vertexRegion in state.vertexData.RegionList)
                {
                    //region = new Region(vertexRegion.Key, vertexRegion.Value, RegionState.VertexInfo);
                    graphicsData.AddRegion(vertexRegion.Key, vertexRegion.Value);
                    //regions.Add(region);
                }

                int count = 0;
                foreach (KeyValuePair<int, int> notFixedRegion in graphicsData.RegionList)
                {
                    region = new DynamicRegion(notFixedRegion.Key, notFixedRegion.Value, RegionState.GraphicsData);
                    region.number = count++;
                    regions.Add(region);
                }


                region = new DisplayListRegion(offset, rom.offset - offset, state.isFogEnabled, state.isEnvColorEnabled, state.FCCount, state.B9cmdfirst, layer);
                regions.Add(region);
            }
            finally
            {
                rom.PopOffset();
            }
            return state;
        }

        public static void PerformRegionRelocation(Region region, RelocationTable table)
        {
            // This is fake rom but it works anyways, just more convenient
            ROM rom = new ROM(region.data);

            byte curCmdIndex;
            do
            {
                curCmdIndex = rom.Read8();
                RelocationParseCmd func = relocationParser[curCmdIndex];
                func(rom, table, (DisplayListRegion) region);
                rom.AddOffset(8);
            }
            while (rom.offset < region.length);
        }

        private static void RegionParse_common(ROM rom, List<Region> regions, RegionParseState state) { }
        private static void RelocationParse_common(ROM rom, RelocationTable table, DisplayListRegion region) { }

        private static void RegionParse_cmd01(ROM rom, List<Region> regions, RegionParseState state)
        {
            throw new NotSupportedException("segmented vectors not supported");
        }

        private static void RegionParse_cmd03(ROM rom, List<Region> regions, RegionParseState state)
        {
            int segment = rom.Read8(4);
            if (segment != 0x0e)
                return;

            int segmentedAddress = rom.Read32(4);
            int address = rom.GetROMAddress(segmentedAddress);

            state.lightData.AddRegion(address, 0x8);
        }

        private static void RelocationParse_cmd03(ROM rom, RelocationTable table, DisplayListRegion region)
        {
            table.RelocateOffset(rom, 4);
        }

        private static void RegionParse_cmd04(ROM rom, List<Region> regions, RegionParseState state)
        {
            int segment = rom.Read8(4);
            if (segment != 0x0e)
                return;

            int segmentedAddress = rom.Read32(4);
            int address = rom.GetROMAddress(segmentedAddress);
            state.vertexData.AddRegion(address, rom.Read16(2));
            //regions.Add(new Region(address, rom.Read16(2), RegionState.VertexInfo));
        }

        private static void RelocationParse_cmd04(ROM rom, RelocationTable table, DisplayListRegion region)
        {
            table.RelocateOffset(rom, 4);
        }

        private static void RegionParse_cmd06(ROM rom, List<Region> regions, RegionParseState state)
        {
            int segment = rom.Read8(4);
            if (segment != 0x0e)
                return;

            int type = rom.Read8(1);
            if (type != 0)
                throw new NotSupportedException("Terminating Branch not supported!");

            int segmentedAddress = rom.Read32(4);
            int address = rom.GetROMAddress(segmentedAddress);

            List<Region> regionsBranch = new List<Region>();
            RegionParseState stateBranch = PerformRegionParseInternal(rom, regionsBranch, address, 0);

            // TODO: Copy/update more if needed
            if (stateBranch.FDAddress != -1)
                state.FDAddress = stateBranch.FDAddress;

            foreach (Region region in regionsBranch)
            {
                state.textureData.AddRegion(region.romStart, region.length);
            }
        }

        private static void RegionParse_cmdB7(ROM rom, List<Region> regions, RegionParseState state)
        {
            if ((rom.Read32(4) & 0x00010000) != 0)
                state.isFogEnabled = true;
        }

        private static void RegionParse_cmdB9(ROM rom, List<Region> regions, RegionParseState state)
        {
            if (state.B9cmdfirst == 0)
                state.B9cmdfirst = rom.Read64();
        }

        private static void RelocationParse_cmdB9(ROM rom, RelocationTable table, DisplayListRegion region)
        {
            if (!region.DLFixesNeeded)
                return;

            if (!region.isFogEnabled)
                return;

            if ((ulong)rom.Read64(-8) != 0xBA00140200000000)
                return;
            
                if (region.layer == 1)
                    rom.Write64(0xB900031D00552078);
                if (region.layer == 4)
                    rom.Write64(0xB900031D00443078);
                if (region.layer == 5)
                    rom.Write64(0xB900031D005049D8);
        }

        private static void RegionParse_cmdF0(ROM rom, List<Region> regions, RegionParseState state)
        {
            if (state.FDAddress == -1)
                return;

            int w0 = rom.Read16(5);
            int colorCount = (w0 >> (4 + 2)) + 1;
            
            state.textureData.AddRegion(state.FDAddress, colorCount * 2);
        }

        private static void RegionParse_cmdF3(ROM rom, List<Region> regions, RegionParseState state)
        {
            if (state.FDAddress == -1)
                return;

            int w0 = (int) rom.Read32(0);
            int w1 = (int) rom.Read32(4);
            int uls = (w0 >> 12) & 0x3FF;
            int ult = w0 & 0x3FF;
            int lrs = (w1 >> 12) & 0x3FF;
            int dxt = w1 & 0x3FF;

            // idk how this actually works
            int textureSize = (lrs + 1) * 4;
            state.textureData.AddRegion(state.FDAddress, textureSize);
        }

        private static void RegionParse_cmdFB(ROM rom, List<Region> regions, RegionParseState state)
        {
            state.isEnvColorEnabled = true;
        }

        private static void RegionParse_cmdFC(ROM rom, List<Region> regions, RegionParseState state)
        {
            state.FCCount++;
        }

        private static void RelocationParse_cmdFC(ROM rom, RelocationTable table, DisplayListRegion region)
        {
            if (!region.DLFixesNeeded)
                return;


            UInt64 FCcmd = 0;
            if (region.FCCountFixed == 0)
            {
                if (region.isFogEnabled)
                {
                    if ((ulong)region.B9cmdfirst == 0xB900031DC8112078)
                        FCcmd = 0xFC127FFFFFFFF838;

                    if ((ulong)region.B9cmdfirst == 0xB900031DC8113078)
                        FCcmd = 0xFCFFFFFFFFFCF238;

                    if (region.isEnvcolorEnabled)
                        FCcmd = 0xFC127FFFFFFFFA38;
                }
                else
                {
                    if (region.FCCount == 1)
                        FCcmd = 0xFC127E24FFFFF9FC;
                    else
                    {
                        if (!region.isEnvcolorEnabled)
                        {
                            FCcmd = 0xFC121824FF33FFFF;
                        }
                        else
                        {
                            FCcmd = 0xFC122E24FFFFFBFD;
                        }
                    }
                }
            }
            else
            {
                FCcmd = 0xFC127E24FFFFF9FC;
            }

            if (FCcmd == 0)
            {
                MessageBox.Show("Parser could not choose the right combiner! Combiners won't be fixed", "Display List", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                region.FCCountFixed++;
                rom.Write64(FCcmd);
            }
        }

        private static void RegionParse_cmdFD(ROM rom, List<Region> regions, RegionParseState state)
        {
            state.FDAddress = -1;

            int segment = rom.Read8(4);
            if (segment != 0x0e)
                return;

            int segmentedAddress = rom.Read32(4);
            int address = (int)rom.GetROMAddress(segmentedAddress);
            state.FDAddress = address;
        }

        private static void RelocationParse_cmdFD(ROM rom, RelocationTable table, DisplayListRegion region)
        {
            table.RelocateOffset(rom, 4);
        }
    }
}
