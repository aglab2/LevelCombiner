namespace LevelCombiner
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitROM = new System.Windows.Forms.Button();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.levelFooterCheckbox = new System.Windows.Forms.CheckBox();
            this.levelHeaderCheckbox = new System.Windows.Forms.CheckBox();
            this.areaHeaderCheckbox = new System.Windows.Forms.CheckBox();
            this.areaFooterCheckbox = new System.Windows.Forms.CheckBox();
            this.areaObjectsDataCheckbox = new System.Windows.Forms.CheckBox();
            this.areaGraphicsCheckBox = new System.Windows.Forms.CheckBox();
            this.modelsDescriptorCheckbox = new System.Windows.Forms.CheckBox();
            this.modelsGraphicsCheckbox = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.splittedPathTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.deleteFolderCheckbox = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.levelCollisionCheckbox = new System.Windows.Forms.CheckBox();
            this.levelsComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.trimObjectsCheckbox = new System.Windows.Forms.CheckBox();
            this.checkBoxOldScrolls = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // splitROM
            // 
            this.splitROM.Location = new System.Drawing.Point(12, 324);
            this.splitROM.Name = "splitROM";
            this.splitROM.Size = new System.Drawing.Size(75, 23);
            this.splitROM.TabIndex = 0;
            this.splitROM.Text = "Split ROM";
            this.splitROM.UseVisualStyleBackColor = true;
            this.splitROM.Click += new System.EventHandler(this.splitROM_Click);
            // 
            // addressTextBox
            // 
            this.addressTextBox.Location = new System.Drawing.Point(86, 12);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(134, 20);
            this.addressTextBox.TabIndex = 1;
            this.addressTextBox.Text = "0x2AC0F8";
            this.addressTextBox.TextChanged += new System.EventHandler(this.addressTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ptr to Level";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Data to Extract";
            // 
            // levelFooterCheckbox
            // 
            this.levelFooterCheckbox.AutoSize = true;
            this.levelFooterCheckbox.Checked = true;
            this.levelFooterCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.levelFooterCheckbox.Location = new System.Drawing.Point(12, 178);
            this.levelFooterCheckbox.Name = "levelFooterCheckbox";
            this.levelFooterCheckbox.Size = new System.Drawing.Size(85, 17);
            this.levelFooterCheckbox.TabIndex = 4;
            this.levelFooterCheckbox.Text = "Level Footer";
            this.levelFooterCheckbox.UseVisualStyleBackColor = true;
            // 
            // levelHeaderCheckbox
            // 
            this.levelHeaderCheckbox.AutoSize = true;
            this.levelHeaderCheckbox.Checked = true;
            this.levelHeaderCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.levelHeaderCheckbox.Location = new System.Drawing.Point(12, 155);
            this.levelHeaderCheckbox.Name = "levelHeaderCheckbox";
            this.levelHeaderCheckbox.Size = new System.Drawing.Size(90, 17);
            this.levelHeaderCheckbox.TabIndex = 5;
            this.levelHeaderCheckbox.Text = "Level Header";
            this.levelHeaderCheckbox.UseVisualStyleBackColor = true;
            // 
            // areaHeaderCheckbox
            // 
            this.areaHeaderCheckbox.AutoSize = true;
            this.areaHeaderCheckbox.Checked = true;
            this.areaHeaderCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.areaHeaderCheckbox.Location = new System.Drawing.Point(123, 155);
            this.areaHeaderCheckbox.Name = "areaHeaderCheckbox";
            this.areaHeaderCheckbox.Size = new System.Drawing.Size(86, 17);
            this.areaHeaderCheckbox.TabIndex = 6;
            this.areaHeaderCheckbox.Text = "Area Header";
            this.areaHeaderCheckbox.UseVisualStyleBackColor = true;
            // 
            // areaFooterCheckbox
            // 
            this.areaFooterCheckbox.AutoSize = true;
            this.areaFooterCheckbox.Checked = true;
            this.areaFooterCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.areaFooterCheckbox.Location = new System.Drawing.Point(123, 178);
            this.areaFooterCheckbox.Name = "areaFooterCheckbox";
            this.areaFooterCheckbox.Size = new System.Drawing.Size(81, 17);
            this.areaFooterCheckbox.TabIndex = 7;
            this.areaFooterCheckbox.Text = "Area Footer";
            this.areaFooterCheckbox.UseVisualStyleBackColor = true;
            // 
            // areaObjectsDataCheckbox
            // 
            this.areaObjectsDataCheckbox.AutoSize = true;
            this.areaObjectsDataCheckbox.Checked = true;
            this.areaObjectsDataCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.areaObjectsDataCheckbox.Location = new System.Drawing.Point(123, 224);
            this.areaObjectsDataCheckbox.Name = "areaObjectsDataCheckbox";
            this.areaObjectsDataCheckbox.Size = new System.Drawing.Size(87, 17);
            this.areaObjectsDataCheckbox.TabIndex = 8;
            this.areaObjectsDataCheckbox.Text = "Area Objects";
            this.areaObjectsDataCheckbox.UseVisualStyleBackColor = true;
            // 
            // areaGraphicsCheckBox
            // 
            this.areaGraphicsCheckBox.AutoSize = true;
            this.areaGraphicsCheckBox.Checked = true;
            this.areaGraphicsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.areaGraphicsCheckBox.Location = new System.Drawing.Point(123, 247);
            this.areaGraphicsCheckBox.Name = "areaGraphicsCheckBox";
            this.areaGraphicsCheckBox.Size = new System.Drawing.Size(93, 17);
            this.areaGraphicsCheckBox.TabIndex = 9;
            this.areaGraphicsCheckBox.Text = "Area Graphics";
            this.areaGraphicsCheckBox.UseVisualStyleBackColor = true;
            // 
            // modelsDescriptorCheckbox
            // 
            this.modelsDescriptorCheckbox.AutoSize = true;
            this.modelsDescriptorCheckbox.Checked = true;
            this.modelsDescriptorCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.modelsDescriptorCheckbox.Location = new System.Drawing.Point(12, 224);
            this.modelsDescriptorCheckbox.Name = "modelsDescriptorCheckbox";
            this.modelsDescriptorCheckbox.Size = new System.Drawing.Size(60, 17);
            this.modelsDescriptorCheckbox.TabIndex = 10;
            this.modelsDescriptorCheckbox.Text = "Models";
            this.modelsDescriptorCheckbox.UseVisualStyleBackColor = true;
            // 
            // modelsGraphicsCheckbox
            // 
            this.modelsGraphicsCheckbox.AutoSize = true;
            this.modelsGraphicsCheckbox.Checked = true;
            this.modelsGraphicsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.modelsGraphicsCheckbox.Location = new System.Drawing.Point(12, 247);
            this.modelsGraphicsCheckbox.Name = "modelsGraphicsCheckbox";
            this.modelsGraphicsCheckbox.Size = new System.Drawing.Size(105, 17);
            this.modelsGraphicsCheckbox.TabIndex = 11;
            this.modelsGraphicsCheckbox.Text = "Models Graphics";
            this.modelsGraphicsCheckbox.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(145, 324);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Build ROM";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // splittedPathTextBox
            // 
            this.splittedPathTextBox.Location = new System.Drawing.Point(86, 65);
            this.splittedPathTextBox.Name = "splittedPathTextBox";
            this.splittedPathTextBox.Size = new System.Drawing.Size(134, 20);
            this.splittedPathTextBox.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Splitted Path";
            // 
            // deleteFolderCheckbox
            // 
            this.deleteFolderCheckbox.AutoSize = true;
            this.deleteFolderCheckbox.Location = new System.Drawing.Point(12, 103);
            this.deleteFolderCheckbox.Name = "deleteFolderCheckbox";
            this.deleteFolderCheckbox.Size = new System.Drawing.Size(161, 17);
            this.deleteFolderCheckbox.TabIndex = 15;
            this.deleteFolderCheckbox.Text = "Clean Path before Extraction";
            this.deleteFolderCheckbox.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(145, 129);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 16;
            this.button2.Text = "Uncheck All";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // levelCollisionCheckbox
            // 
            this.levelCollisionCheckbox.AutoSize = true;
            this.levelCollisionCheckbox.Checked = true;
            this.levelCollisionCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.levelCollisionCheckbox.Location = new System.Drawing.Point(123, 201);
            this.levelCollisionCheckbox.Name = "levelCollisionCheckbox";
            this.levelCollisionCheckbox.Size = new System.Drawing.Size(89, 17);
            this.levelCollisionCheckbox.TabIndex = 17;
            this.levelCollisionCheckbox.Text = "Area Collision";
            this.levelCollisionCheckbox.UseVisualStyleBackColor = true;
            // 
            // levelsComboBox
            // 
            this.levelsComboBox.FormattingEnabled = true;
            this.levelsComboBox.Items.AddRange(new object[] {
            "Course 1",
            "Course 2",
            "Course 3",
            "Course 4",
            "Course 5",
            "Course 6",
            "Course 7",
            "Course 8",
            "Course 9",
            "Course 10",
            "Course 11",
            "Course 12",
            "Course 13",
            "Course 14",
            "Course 15",
            "Bowser Course 1",
            "Bowser Battle 1",
            "Bowser Course 2",
            "Bowser Battle 2",
            "Bowser Course 3",
            "Bowser Battle 3",
            "Peach Slide",
            "Metal Cap",
            "Wing Cap",
            "Vanish Cap",
            "Rainbow",
            "Aquarium",
            "\"The End\"",
            "Castle Grounds",
            "Inside Castle",
            "Castle Courtyard"});
            this.levelsComboBox.Location = new System.Drawing.Point(86, 38);
            this.levelsComboBox.Name = "levelsComboBox";
            this.levelsComboBox.Size = new System.Drawing.Size(134, 21);
            this.levelsComboBox.TabIndex = 19;
            this.levelsComboBox.SelectedIndexChanged += new System.EventHandler(this.levelsComboBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Level Name";
            // 
            // trimObjectsCheckbox
            // 
            this.trimObjectsCheckbox.AutoSize = true;
            this.trimObjectsCheckbox.Location = new System.Drawing.Point(12, 281);
            this.trimObjectsCheckbox.Name = "trimObjectsCheckbox";
            this.trimObjectsCheckbox.Size = new System.Drawing.Size(85, 17);
            this.trimObjectsCheckbox.TabIndex = 21;
            this.trimObjectsCheckbox.Text = "Trim Objects";
            this.trimObjectsCheckbox.UseVisualStyleBackColor = true;
            // 
            // checkBoxOldScrolls
            // 
            this.checkBoxOldScrolls.AutoSize = true;
            this.checkBoxOldScrolls.Location = new System.Drawing.Point(123, 281);
            this.checkBoxOldScrolls.Name = "checkBoxOldScrolls";
            this.checkBoxOldScrolls.Size = new System.Drawing.Size(91, 17);
            this.checkBoxOldScrolls.TabIndex = 22;
            this.checkBoxOldScrolls.Text = "1.9.3S Scrolls";
            this.checkBoxOldScrolls.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 359);
            this.Controls.Add(this.checkBoxOldScrolls);
            this.Controls.Add(this.trimObjectsCheckbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.levelsComboBox);
            this.Controls.Add(this.levelCollisionCheckbox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.deleteFolderCheckbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.splittedPathTextBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.modelsGraphicsCheckbox);
            this.Controls.Add(this.modelsDescriptorCheckbox);
            this.Controls.Add(this.areaGraphicsCheckBox);
            this.Controls.Add(this.areaObjectsDataCheckbox);
            this.Controls.Add(this.areaFooterCheckbox);
            this.Controls.Add(this.areaHeaderCheckbox);
            this.Controls.Add(this.levelHeaderCheckbox);
            this.Controls.Add(this.levelFooterCheckbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.addressTextBox);
            this.Controls.Add(this.splitROM);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Level Combiner";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button splitROM;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox levelFooterCheckbox;
        private System.Windows.Forms.CheckBox levelHeaderCheckbox;
        private System.Windows.Forms.CheckBox areaHeaderCheckbox;
        private System.Windows.Forms.CheckBox areaFooterCheckbox;
        private System.Windows.Forms.CheckBox areaObjectsDataCheckbox;
        private System.Windows.Forms.CheckBox areaGraphicsCheckBox;
        private System.Windows.Forms.CheckBox modelsDescriptorCheckbox;
        private System.Windows.Forms.CheckBox modelsGraphicsCheckbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox splittedPathTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox deleteFolderCheckbox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox levelCollisionCheckbox;
        private System.Windows.Forms.ComboBox levelsComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox trimObjectsCheckbox;
        private System.Windows.Forms.CheckBox checkBoxOldScrolls;
    }
}

