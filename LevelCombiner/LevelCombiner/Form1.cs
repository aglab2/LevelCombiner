using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelCombiner
{
    public partial class Form1 : Form
    {
        const int staticRelocationAddress = 0x00800000;
        const int staticRelocationAddress0E = 0x0E000000 | staticRelocationAddress;
        const int staticRelocationAddress19 = 0x19000000 | staticRelocationAddress;

        public Form1()
        {
            InitializeComponent();
        }

        public static string GetTab(RegionState state)
        {
            switch (state)
            {
                case RegionState.LevelHeader:
                case RegionState.LevelFooter:
                case RegionState.ModelsLoader:
                    return "";
                case RegionState.AreaHeader:
                case RegionState.AreaData:
                case RegionState.AreaFooter:
                    return "\t";
                case RegionState.GeoLayout:
                    return "\t\t";
                case RegionState.Collision:
                case RegionState.DisplayList:
                    return "\t\t\t";
                case RegionState.TextureInfo:
                case RegionState.VertexInfo:
                case RegionState.LightData:
                case RegionState.GraphicsData:
                    return "\t\t\t\t";
            }

            return "";
        }

        private void splitROM_Click(object sender, EventArgs e)
        {
            GC.Collect();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ROM File|*.z64";
            openFileDialog1.Title = "Select a ROM";
            
            if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            string path = openFileDialog1.FileName;
            path = Path.GetFullPath(path);

            ROM rom = new ROM(File.ReadAllBytes(path));
            int offset = Convert.ToInt32(addressTextBox.Text, 16);
            if (offset == 0)
            {
                MessageBox.Show("Failed to parse address, input correct address", "Level Split", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<Region> regions = new List<Region>();
            
            // 1st pass : find out where regions are
            LevelScript.PerformRegionParse(rom, regions, offset);

            foreach (Region region in regions)
            {
                Console.WriteLine(GetTab(region.state) + "Region {0:x} {1:x} {2:x}", region.romStart, region.romStart + region.length, region.state.ToString());
            }

            // 2nd pass : with known regions, relocate data
            // GraphicsData position independent
            // Collision    position independent
            // DisplayList to 0E000000 of GraphicsData
            // GeoLayout   to 0E000000 of DisplayList
            // AreaHeader  to 19000000 of GeoLayout
            // ObjectsLoad to 19000000 of GeoLayout
            // AreaFooter  to 0E000000 of Collision
            StaticRelocationTable table = new StaticRelocationTable();
            foreach (Region region in regions)
            {
                RelocationUnit unit = new RelocationUnit(region, rom, isFromStatic: false);
                table.AddUnit(unit);
            }

            // Fill in data from rom
            foreach (Region region in regions)
            {
                region.data = new byte[region.length];
                rom.ReadData(region.romStart, region.length, region.data);
            }

            SortedSet<int> areasTrimmed = new SortedSet<int>();
            // Trim AreaData if flag is checked
            foreach (Region region in regions)
            {
                if (region.state != RegionState.AreaData && region.state != RegionState.AreaScrolls)
                    continue;

                ObjectsTrimmer.Trim(region, !areasTrimmed.Contains(region.area));
                areasTrimmed.Add(region.area);
            }

            // Relocate!
            foreach (Region region in regions)
            {
                try
                {
                    region.Relocate(table);
                }
                catch (ArgumentException)
                {
                    string answer = String.Format("Region relocation failed for {0}", region.state);
                    if (region.area != -1)
                        answer += String.Format(", area {0}", region.area);
                    if (region.model != -1)
                        answer += String.Format(", model {0}", region.model);
                    answer += ", region might be broken";

                    MessageBox.Show(answer, "Level Split", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            Dictionary<RegionState, int> dictionary = new Dictionary<RegionState, int>();
            foreach (RegionState state in (RegionState[])Enum.GetValues(typeof(RegionState)))
            {
                dictionary[state] = 0;
            }

            string dirPostfix = addressTextBox.Text;
            int levelScriptEntryPoint = Convert.ToInt32(addressTextBox.Text, 16);
            if (LevelInfo.IsValidLevelScriptEntry(levelScriptEntryPoint))
                dirPostfix = (LevelInfo.GetLevel(levelScriptEntryPoint) + 1).ToString();

            string dirname = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path), dirPostfix);
            if (deleteFolderCheckbox.Checked)
            {
                if (Directory.Exists(dirname))
                {
                    DialogResult result = MessageBox.Show(String.Format("Path {0} already exists! Delete it anyways?", dirname), "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    if (result == DialogResult.OK)
                        Directory.Delete(dirname, true);
                    else
                        return;
                }
            }

            Directory.CreateDirectory(dirname);
            SortedSet<string> truncatedFiles = new SortedSet<string>();

            foreach (Region region in regions)
            {
                switch (region.state)
                {
                    case RegionState.LevelHeader:
                        if (!levelHeaderCheckbox.Checked)
                            continue;
                        break;
                    case RegionState.LevelFooter:
                        if (!levelFooterCheckbox.Checked)
                            continue;
                        break;
                    case RegionState.ModelsLoader:
                        if (!modelsDescriptorCheckbox.Checked)
                            continue;
                        break;
                    case RegionState.AreaHeader:
                        if (!areaHeaderCheckbox.Checked)
                            continue;
                        break;
                    case RegionState.AreaData:
                    case RegionState.AreaScrolls:
                        if (!areaObjectsDataCheckbox.Checked)
                            continue;
                        break;
                    case RegionState.AreaFooter:
                        if (!areaFooterCheckbox.Checked)
                            continue;
                        break;
                    case RegionState.GeoLayout:
                    case RegionState.DisplayList:
                    case RegionState.GraphicsData:
                        if (region.area != -1)
                        {
                            if (!areaGraphicsCheckBox.Checked)
                                continue;
                        }
                        else if (region.model != -1)
                        {
                            if (!modelsGraphicsCheckbox.Checked)
                                continue;
                        }
                        break;
                    case RegionState.Collision:
                        if (!levelCollisionCheckbox.Checked)
                            continue;
                        break;
                }

                string regionPath = null;
                try
                {
                    regionPath = PathComposer.ComposeName(dirname, region.state, region.area, region.model, region.number);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Failed to compose path for {0}, reason: {1}", region.state, ex.Message), "Level Split", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (regionPath == null)
                    continue;

                Directory.CreateDirectory(Path.GetDirectoryName(regionPath));

                //if (deleteFolderCheckbox.Checked && File.Exists(regionPath))
                //    throw new Exception("Such region was written already!");

                if (!truncatedFiles.Contains(regionPath))
                {
                    File.Delete(regionPath);
                    truncatedFiles.Add(regionPath);
                }
                else
                {
                    if (region.state != RegionState.ModelsLoader && 
                        region.state != RegionState.AreaData && 
                        region.state != RegionState.AreaScrolls)
                        MessageBox.Show(String.Format("Appending data from region {0}, this might be bug", region.state), "Level Split", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                using (var stream = new FileStream(regionPath, FileMode.Append))
                    stream.Write(region.data, 0, region.data.Length);
            }
            
            MessageBox.Show(String.Format("ROM was splitted successfully at {0}", dirname), "Level Split", MessageBoxButtons.OK, MessageBoxIcon.Information);
            splittedPathTextBox.Text = dirname;
            //regions.Sort((x, y) => x.start.CompareTo(y.start));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if ((!levelHeaderCheckbox.Checked)
             && (!levelFooterCheckbox.Checked)
             && (!modelsDescriptorCheckbox.Checked)
             && (!areaHeaderCheckbox.Checked)
             && (!areaObjectsDataCheckbox.Checked)
             && (!areaFooterCheckbox.Checked)
             && (!areaGraphicsCheckBox.Checked)
             && (!modelsGraphicsCheckbox.Checked)
             && (!levelCollisionCheckbox.Checked))
            {
                levelHeaderCheckbox.Checked = true;
                levelFooterCheckbox.Checked = true;
                modelsDescriptorCheckbox.Checked = true;
                areaHeaderCheckbox.Checked = true;
                areaObjectsDataCheckbox.Checked = true;
                areaFooterCheckbox.Checked = true;
                areaGraphicsCheckBox.Checked = true;
                modelsGraphicsCheckbox.Checked = true;
                levelCollisionCheckbox.Checked = true;
                button2.Text = "Uncheck All";
            }
            else
            {
                levelHeaderCheckbox.Checked = false;
                levelFooterCheckbox.Checked = false;
                modelsDescriptorCheckbox.Checked = false;
                areaHeaderCheckbox.Checked = false;
                areaObjectsDataCheckbox.Checked = false;
                areaFooterCheckbox.Checked = false;
                areaGraphicsCheckBox.Checked = false;
                modelsGraphicsCheckbox.Checked = false;
                levelCollisionCheckbox.Checked = false;
                button2.Text = "Check All";
            }
        }

        private void addressTextBox_TextChanged(object sender, EventArgs e)
        {
            int level = 0;

            int levelScriptEntryPoint = Convert.ToInt32(addressTextBox.Text, 16);
            if (!LevelInfo.IsValidLevelScriptEntry(levelScriptEntryPoint))
                return;

            level = LevelInfo.GetLevel(levelScriptEntryPoint);
            levelsComboBox.SelectedIndex = level;
        }

        private void levelsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            addressTextBox.Text = "0x" + Convert.ToString(LevelInfo.GetLevelScriptEntryPoint(levelsComboBox.SelectedIndex), 16).ToUpper();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GC.Collect();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ROM File|*.z64";
            openFileDialog1.Title = "Select a ROM";

            if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            string path = openFileDialog1.FileName;
            path = Path.GetFullPath(path);

            ROM rom = new ROM(File.ReadAllBytes(path));
            int offset = Convert.ToInt32(addressTextBox.Text, 16);
            if (offset == 0)
            {
                MessageBox.Show("Failed to parse address, input correct address", "Level Split", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LevelScript.PerformHeaderParse(rom, offset);

            SegmentDescriptor segmentDescriptor0E = rom.GetSegmentDescriptor(0x0E);
            SegmentDescriptor segmentDescriptor19 = rom.GetSegmentDescriptor(0x19);
            
            DataBuilder segment0E = new DataBuilder(segmentDescriptor0E.start, segmentDescriptor0E.length);
            DataBuilder segment19 = new DataBuilder(segmentDescriptor19.start, segmentDescriptor19.length);

            string dirname = splittedPathTextBox.Text;

            // Fill in area relocation table + geolayouts
            KeyRelocationTable areaGraphicsDescrRelocationTable  = new KeyRelocationTable();
            KeyRelocationTable areaGraphicsDataRelocationTable = new KeyRelocationTable();
            KeyRelocationTable areaCollisionRelocationTable = new KeyRelocationTable();
            for (int area = 1; area <= 8; area++)
            {
                if (!PathComposer.IsRegionFileExists(dirname, RegionState.GraphicsData, area: area))
                    continue;

                // Area graphics
                ParseGraphics(dirname, segment0E, segment19, rom, out RelocationUnit descrUnit, out RelocationUnit dataUnit, area: area);
                areaGraphicsDescrRelocationTable.AddUnit(area, descrUnit);
                areaGraphicsDataRelocationTable.AddUnit(area, dataUnit);

                // Area collision
                DynamicRegion collision = new DynamicRegion(dirname, RegionState.Collision, area: area);
                // No relocation needed
                segment0E.AddRegion(collision);
                segment0E.RoundOffset();

                RelocationUnit collisionRelocationUnit = new RelocationUnit(collision, rom, isFromStatic: true);
                areaCollisionRelocationTable.AddUnit(area, collisionRelocationUnit);
            }

            // Fill in model relocation table + geolayouts
            KeyRelocationTable modelRelocationTable = new KeyRelocationTable();
            for (int model = 0x00; model < 0xFF; model++)
            {
                if (!PathComposer.IsRegionFileExists(dirname, RegionState.GraphicsData, model: model))
                    continue;

                ParseGraphics(dirname, segment0E, segment19, rom, out RelocationUnit descrUnit, out RelocationUnit graphicsUnit, model: model);
                modelRelocationTable.AddUnit(model, descrUnit);
            }

            // As everything is prepared, we can finally start building level!
            LevelScriptRegion levelHeader = new LevelScriptRegion(dirname, RegionState.LevelHeader);
            LevelScript.FixLoadAddresses(rom, levelHeader);
            segment19.AddRegion(levelHeader);

            int levelScriptSegmentedAddressStart = rom.GetSegmentedAddress(levelHeader.romStart);

            LevelScriptRegion modelsLoader = new LevelScriptRegion(dirname, RegionState.ModelsLoader);
            modelsLoader.Relocate(modelRelocationTable);
            segment19.AddRegion(modelsLoader);

            for (sbyte area = 0; area < 8; area++)
            {
                if (!PathComposer.IsRegionFileExists(dirname, RegionState.GraphicsData, area: area))
                    continue;

                // Area header initializes graphics
                LevelScriptRegion areaHeader = new LevelScriptRegion(dirname, RegionState.AreaHeader, area: area);
                LevelScript.PerformRegionRelocation(areaHeader, areaGraphicsDescrRelocationTable, area);
                segment19.AddRegion(areaHeader);

                LevelScriptRegion areaData = new LevelScriptRegion(dirname, RegionState.AreaData, area: area);
                // No relocation needed
                segment19.AddRegion(areaData);

                if (PathComposer.IsRegionFileExists(dirname, RegionState.AreaScrolls, area: area))
                {
                    LevelScriptRegion scrollsData = new LevelScriptRegion(dirname, RegionState.AreaScrolls, area: area);
                    LevelScript.PerformRegionRelocation(scrollsData, areaGraphicsDataRelocationTable, area);
                    segment19.AddRegion(scrollsData);
                }

                // Area footer initializes collision
                LevelScriptRegion areaFooter = new LevelScriptRegion(dirname, RegionState.AreaFooter, area: area);
                LevelScript.PerformRegionRelocation(areaFooter, areaCollisionRelocationTable, area);
                segment19.AddRegion(areaFooter);
            }

            LevelScriptRegion levelFooter = new LevelScriptRegion(dirname, RegionState.LevelFooter);
            // no relocation needed
            segment19.AddRegion(levelFooter);


            // At this point we know that all data fit in rom
            // So just write all that in rom
            using (Stream stream = new FileStream(path, FileMode.Open))
            {
                stream.Seek(segmentDescriptor0E.start, SeekOrigin.Begin);
                stream.Write(segment0E.Data, 0, segment0E.Offset);

                stream.Seek(segmentDescriptor19.start, SeekOrigin.Begin);
                stream.Write(segment19.Data, 0, segment19.Offset);

                // Also start of level script moved so write that thing too
                int endianData = IPAddress.HostToNetworkOrder(levelScriptSegmentedAddressStart);
                byte[] convertedData = BitConverter.GetBytes(endianData);

                stream.Seek(rom.levelScriptEntryOffset, SeekOrigin.Begin);
                stream.Write(convertedData, 0, 4);
            }

            MessageBox.Show(String.Format("ROM was build successfully from {0}", dirname), "Level Split", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ParseGraphics(string dirname, DataBuilder segment0E, DataBuilder segmentGeoLayouts, ROM rom, out RelocationUnit unit, out RelocationUnit graphicsDataUnit, int area = -1, int model = -1)
        {
            RelocationUnit retValue = null;
            RelocationUnit graphicsUnit = null;

            segmentGeoLayouts.Backup();
            segment0E.Backup();
            try
            {
                DynamicRegion graphicsDataRegion = new DynamicRegion(dirname, RegionState.GraphicsData, area, model);
                // no relocation needed for dynamic region
                segment0E.AddRegion(graphicsDataRegion);
                segment0E.RoundOffset();

                // Display lists needs to be relocated with static graphics relocation table
                StaticRelocationTable graphicsRelocationTable = new StaticRelocationTable();
                graphicsUnit = new RelocationUnit(graphicsDataRegion, rom, isFromStatic: true);
                graphicsRelocationTable.AddUnit(graphicsUnit);

                // Geolayouts needs to be relocated with queued display lists, will be filled during relocation with graphicsRelocationTable
                QueueRelocationTable dispRelocationTable = new QueueRelocationTable();

                RelocationUnit dispRelocationUnit = null;
                for (int dispNumber = 0; dispNumber < 0xFF; dispNumber++)
                {
                    if (!PathComposer.IsRegionFileExists(dirname, RegionState.DisplayList, area, model, dispNumber))
                        break;

                    DisplayListRegion dispRegion = new DisplayListRegion(dirname, area, model, dispNumber);
                    DisplayList.PerformRegionRelocation(dispRegion, graphicsRelocationTable);
                    segment0E.AddRegion(dispRegion);
                    segment0E.RoundOffset();

                    dispRelocationUnit = new RelocationUnit(dispRegion, rom, isFromStatic: true);
                    dispRelocationTable.AddUnit(dispRelocationUnit);
                }

                // Not even one disp relocation unit, sounds like a bug
                if (dispRelocationUnit == null)
                    throw new IOException("No display lists found!");

                // Geolayout might or might not exist for model, check if it exists and if needed, relocate it
                if (PathComposer.IsRegionFileExists(dirname, RegionState.GeoLayout, area, model))
                {
                    // Load geolayout and relocate it with display lists
                    GeoLayoutRegion modelGeoLayoutRegion = new GeoLayoutRegion(dirname, area, model);
                    GeoLayout.PerformRegionRelocation(modelGeoLayoutRegion, dispRelocationTable);
                    segmentGeoLayouts.AddRegion(modelGeoLayoutRegion);
                    segmentGeoLayouts.RoundOffset();

                    // Finalize with returning geolayout for model
                    retValue = new RelocationUnit(modelGeoLayoutRegion, rom, isFromStatic: true);
                }
                else
                {
                    // Return display list only, there should be only one, if more, it is undefinied behaviour :3
                    retValue = dispRelocationUnit;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Failed to load model {0}, reason : '{1}'", model, ex.Message), "Level Combiner", MessageBoxButtons.OK, MessageBoxIcon.Error);
                segment0E.Restore();
                segmentGeoLayouts.Restore();
            }

            unit = retValue;
            graphicsDataUnit = graphicsUnit;
        }
    }
}
