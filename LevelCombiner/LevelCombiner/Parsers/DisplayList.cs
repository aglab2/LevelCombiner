using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LevelCombiner
{
    class DisplayList
    {
        delegate void RegionParseCmd(ROM rom, List<Region> regions, RegionParseState state);
        delegate void RelocationParseCmd(ROM rom, RelocationTable table);

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

            public RegionParseState()
            {
                textureData = new SortedRegionList();
                vertexData = new SortedRegionList();
                lightData = new SortedRegionList();

                retAddressStack = new Stack<int>();
            }
        }

        public static void PerformRegionParse(ROM rom, List<Region> regions, int offset)
        {
            rom.PushOffset(offset);
            try
            {
                RegionParseState state = new RegionParseState();
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


                region = new DisplayListRegion(offset, rom.offset - offset);
                regions.Add(region);
            }
            finally
            {
                rom.PopOffset();
            }
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
                func(rom, table);
                rom.AddOffset(8);
            }
            while (rom.offset < region.length);
        }

        private static void RegionParse_common(ROM rom, List<Region> regions, RegionParseState state) { }
        private static void RelocationParse_common(ROM rom, RelocationTable table) { }

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

        private static void RelocationParse_cmd03(ROM rom, RelocationTable table)
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

        private static void RelocationParse_cmd04(ROM rom, RelocationTable table)
        {
            table.RelocateOffset(rom, 4);
        }

        private static void RegionParse_cmd06(ROM rom, List<Region> regions, RegionParseState state)
        {
            throw new NotSupportedException("Branch not supported, sm64 level importer from 1.9.3S to 2.0.9 or area importer 2.X supported");
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

        private static void RelocationParse_cmdFD(ROM rom, RelocationTable table)
        {
            table.RelocateOffset(rom, 4);
        }
    }
}
