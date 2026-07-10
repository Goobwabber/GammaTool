using System.Drawing;
using System.Windows.Forms;

namespace rebellagamma
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Label labelGamma;
        private TrackBar trackBarGamma;
        private Label labelBrightness;
        private TrackBar trackBarBrightness;
        private Label labelContrast;
        private System.Windows.Forms.TrackBar trackBarContrast;
        private ComboBox comboBoxMonitors;
        private Button buttonReset;
        private Panel panelGraph;
        private System.Windows.Forms.NumericUpDown numericUpDownTargetAR;
        private System.Windows.Forms.Button buttonSaveAR1;
        private System.Windows.Forms.Button buttonSaveAR2;
        private System.Windows.Forms.Label labelAR1;
        private System.Windows.Forms.Label labelAR2;
        private System.Windows.Forms.Button buttonSetAR3;

        // New profile controls
        private Button buttonSaveProfile;
        private Button buttonLoadProfile;
        private TextBox textBoxProfileName;
        private ComboBox comboBoxProfiles;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelGamma = new System.Windows.Forms.Label();
            trackBarGamma = new System.Windows.Forms.TrackBar();
            labelBrightness = new System.Windows.Forms.Label();
            trackBarBrightness = new System.Windows.Forms.TrackBar();
            labelContrast = new System.Windows.Forms.Label();
            trackBarContrast = new System.Windows.Forms.TrackBar();
            comboBoxMonitors = new System.Windows.Forms.ComboBox();
            buttonReset = new System.Windows.Forms.Button();
            panelGraph = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            buttonSaveAR1 = new System.Windows.Forms.Button();
            buttonSaveAR2 = new System.Windows.Forms.Button();
            labelAR1 = new System.Windows.Forms.Label();
            labelAR2 = new System.Windows.Forms.Label();
            buttonSetAR3 = new System.Windows.Forms.Button();
            buttonSaveProfile = new System.Windows.Forms.Button();
            buttonLoadProfile = new System.Windows.Forms.Button();
            textBoxProfileName = new System.Windows.Forms.TextBox();
            comboBoxProfiles = new System.Windows.Forms.ComboBox();
            numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            numericUpDownTargetAR = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            trackBarAR = new System.Windows.Forms.TrackBar();
            labelConnected = new System.Windows.Forms.Label();
            labelTosuInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)trackBarGamma).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarContrast).BeginInit();
            panelGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTargetAR).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarAR).BeginInit();
            SuspendLayout();
            // 
            // labelGamma
            // 
            labelGamma.AutoSize = true;
            labelGamma.ForeColor = System.Drawing.Color.White;
            labelGamma.Location = new System.Drawing.Point(7, 13);
            labelGamma.Name = "labelGamma";
            labelGamma.Size = new System.Drawing.Size(49, 15);
            labelGamma.TabIndex = 0;
            labelGamma.Text = "Gamma";
            // 
            // trackBarGamma
            // 
            trackBarGamma.AutoSize = false;
            trackBarGamma.BackColor = System.Drawing.Color.Black;
            trackBarGamma.LargeChange = 10;
            trackBarGamma.Location = new System.Drawing.Point(117, 11);
            trackBarGamma.Maximum = 888;
            trackBarGamma.Minimum = 8;
            trackBarGamma.Name = "trackBarGamma";
            trackBarGamma.Size = new System.Drawing.Size(160, 45);
            trackBarGamma.TabIndex = 2;
            trackBarGamma.TickFrequency = 100;
            trackBarGamma.TickStyle = System.Windows.Forms.TickStyle.None;
            trackBarGamma.Value = 100;
            trackBarGamma.Scroll += trackBarGamma_Scroll;
            trackBarGamma.MouseDown += trackBar_MouseDown_RemoveFocus;
            // 
            // labelBrightness
            // 
            labelBrightness.AutoSize = true;
            labelBrightness.ForeColor = System.Drawing.Color.White;
            labelBrightness.Location = new System.Drawing.Point(7, 42);
            labelBrightness.Name = "labelBrightness";
            labelBrightness.Size = new System.Drawing.Size(62, 15);
            labelBrightness.TabIndex = 3;
            labelBrightness.Text = "Brightness";
            // 
            // trackBarBrightness
            // 
            trackBarBrightness.BackColor = System.Drawing.Color.Black;
            trackBarBrightness.Location = new System.Drawing.Point(117, 40);
            trackBarBrightness.Maximum = 888;
            trackBarBrightness.Minimum = -888;
            trackBarBrightness.Name = "trackBarBrightness";
            trackBarBrightness.Size = new System.Drawing.Size(160, 45);
            trackBarBrightness.TabIndex = 5;
            trackBarBrightness.TickStyle = System.Windows.Forms.TickStyle.None;
            trackBarBrightness.Scroll += trackBarBrightness_Scroll;
            trackBarBrightness.MouseDown += trackBar_MouseDown_RemoveFocus;
            // 
            // labelContrast
            // 
            labelContrast.AutoSize = true;
            labelContrast.ForeColor = System.Drawing.Color.White;
            labelContrast.Location = new System.Drawing.Point(7, 71);
            labelContrast.Name = "labelContrast";
            labelContrast.Size = new System.Drawing.Size(52, 15);
            labelContrast.TabIndex = 6;
            labelContrast.Text = "Contrast";
            // 
            // trackBarContrast
            // 
            trackBarContrast.BackColor = System.Drawing.Color.Black;
            trackBarContrast.Location = new System.Drawing.Point(117, 69);
            trackBarContrast.Maximum = 888;
            trackBarContrast.Minimum = 8;
            trackBarContrast.Name = "trackBarContrast";
            trackBarContrast.Size = new System.Drawing.Size(160, 45);
            trackBarContrast.TabIndex = 8;
            trackBarContrast.TickFrequency = 10;
            trackBarContrast.TickStyle = System.Windows.Forms.TickStyle.None;
            trackBarContrast.Value = 100;
            trackBarContrast.Scroll += trackBarContrast_Scroll;
            trackBarContrast.MouseDown += trackBar_MouseDown_RemoveFocus;
            // 
            // comboBoxMonitors
            // 
            comboBoxMonitors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxMonitors.Location = new System.Drawing.Point(275, 114);
            comboBoxMonitors.Name = "comboBoxMonitors";
            comboBoxMonitors.Size = new System.Drawing.Size(100, 23);
            comboBoxMonitors.TabIndex = 9;
            comboBoxMonitors.SelectedIndexChanged += comboBoxMonitors_SelectedIndexChanged;
            // 
            // buttonReset
            // 
            buttonReset.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            buttonReset.Location = new System.Drawing.Point(275, 142);
            buttonReset.Name = "buttonReset";
            buttonReset.Size = new System.Drawing.Size(100, 43);
            buttonReset.TabIndex = 10;
            buttonReset.Text = "⛧ Reset ⛧";
            buttonReset.Click += buttonReset_Click;
            // 
            // panelGraph
            // 
            panelGraph.BackColor = System.Drawing.Color.Black;
            panelGraph.Controls.Add(label1);
            panelGraph.Location = new System.Drawing.Point(275, 7);
            panelGraph.Name = "panelGraph";
            panelGraph.Size = new System.Drawing.Size(100, 100);
            panelGraph.TabIndex = 11;
            panelGraph.Paint += panelGraph_Paint;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.Font = new System.Drawing.Font("Segoe UI", 6F);
            label1.ForeColor = System.Drawing.Color.FromArgb(((int)((byte)8)), ((int)((byte)8)), ((int)((byte)8)));
            label1.Location = new System.Drawing.Point(70, 87);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(29, 11);
            label1.TabIndex = 30;
            label1.Text = "rebella";
            // 
            // buttonSaveAR1
            // 
            buttonSaveAR1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            buttonSaveAR1.Location = new System.Drawing.Point(7, 126);
            buttonSaveAR1.Name = "buttonSaveAR1";
            buttonSaveAR1.Size = new System.Drawing.Size(82, 27);
            buttonSaveAR1.TabIndex = 14;
            buttonSaveAR1.Text = "Save Min AR";
            buttonSaveAR1.Click += buttonSaveAR1_Click;
            // 
            // buttonSaveAR2
            // 
            buttonSaveAR2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            buttonSaveAR2.Location = new System.Drawing.Point(7, 156);
            buttonSaveAR2.Name = "buttonSaveAR2";
            buttonSaveAR2.Size = new System.Drawing.Size(82, 27);
            buttonSaveAR2.TabIndex = 18;
            buttonSaveAR2.Text = "Save Max AR";
            buttonSaveAR2.Click += buttonSaveAR2_Click;
            // 
            // labelAR1
            // 
            labelAR1.AutoSize = true;
            labelAR1.Font = new System.Drawing.Font("Segoe UI", 6.4F);
            labelAR1.ForeColor = System.Drawing.Color.White;
            labelAR1.Location = new System.Drawing.Point(96, 134);
            labelAR1.Name = "labelAR1";
            labelAR1.Size = new System.Drawing.Size(41, 12);
            labelAR1.TabIndex = 15;
            labelAR1.Text = "Min AR: -";
            // 
            // labelAR2
            // 
            labelAR2.AutoSize = true;
            labelAR2.Font = new System.Drawing.Font("Segoe UI", 6.4F);
            labelAR2.ForeColor = System.Drawing.Color.White;
            labelAR2.Location = new System.Drawing.Point(95, 164);
            labelAR2.Name = "labelAR2";
            labelAR2.Size = new System.Drawing.Size(43, 12);
            labelAR2.TabIndex = 19;
            labelAR2.Text = "Max AR: -";
            // 
            // buttonSetAR3
            // 
            buttonSetAR3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            buttonSetAR3.Location = new System.Drawing.Point(231, 95);
            buttonSetAR3.Name = "buttonSetAR3";
            buttonSetAR3.Size = new System.Drawing.Size(38, 27);
            buttonSetAR3.TabIndex = 22;
            buttonSetAR3.Text = "Calc";
            buttonSetAR3.Click += buttonSetAR3_Click;
            // 
            // buttonSaveProfile
            // 
            buttonSaveProfile.Font = new System.Drawing.Font("Segoe UI", 9F);
            buttonSaveProfile.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            buttonSaveProfile.Location = new System.Drawing.Point(7, 188);
            buttonSaveProfile.Name = "buttonSaveProfile";
            buttonSaveProfile.Size = new System.Drawing.Size(75, 27);
            buttonSaveProfile.TabIndex = 25;
            buttonSaveProfile.Text = "Save";
            buttonSaveProfile.Click += buttonSaveProfile_Click;
            // 
            // buttonLoadProfile
            // 
            buttonLoadProfile.Font = new System.Drawing.Font("Segoe UI", 9F);
            buttonLoadProfile.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            buttonLoadProfile.Location = new System.Drawing.Point(194, 188);
            buttonLoadProfile.Name = "buttonLoadProfile";
            buttonLoadProfile.Size = new System.Drawing.Size(75, 27);
            buttonLoadProfile.TabIndex = 26;
            buttonLoadProfile.Text = "Load";
            buttonLoadProfile.Click += buttonLoadProfile_Click;
            // 
            // textBoxProfileName
            // 
            textBoxProfileName.Location = new System.Drawing.Point(88, 190);
            textBoxProfileName.Name = "textBoxProfileName";
            textBoxProfileName.Size = new System.Drawing.Size(100, 23);
            textBoxProfileName.TabIndex = 27;
            textBoxProfileName.KeyDown += textBoxProfileName_KeyDown;
            // 
            // comboBoxProfiles
            // 
            comboBoxProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxProfiles.FormattingEnabled = true;
            comboBoxProfiles.Location = new System.Drawing.Point(275, 190);
            comboBoxProfiles.Name = "comboBoxProfiles";
            comboBoxProfiles.Size = new System.Drawing.Size(100, 23);
            comboBoxProfiles.TabIndex = 29;
            // 
            // numericUpDown1
            // 
            numericUpDown1.DecimalPlaces = 2;
            numericUpDown1.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numericUpDown1.Location = new System.Drawing.Point(72, 10);
            numericUpDown1.Maximum = new decimal(new int[] { 888, 0, 0, 131072 });
            numericUpDown1.Minimum = new decimal(new int[] { 8, 0, 0, 131072 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new System.Drawing.Size(46, 23);
            numericUpDown1.TabIndex = 30;
            numericUpDown1.Value = new decimal(new int[] { 8, 0, 0, 131072 });
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            numericUpDown1.KeyDown += numericUpDownKeyDown;
            numericUpDown1.KeyPress += numericUpDownKeyPress;
            // 
            // numericUpDown2
            // 
            numericUpDown2.DecimalPlaces = 2;
            numericUpDown2.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numericUpDown2.Location = new System.Drawing.Point(72, 39);
            numericUpDown2.Maximum = new decimal(new int[] { 888, 0, 0, 131072 });
            numericUpDown2.Minimum = new decimal(new int[] { 888, 0, 0, -2147352576 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new System.Drawing.Size(46, 23);
            numericUpDown2.TabIndex = 31;
            numericUpDown2.ValueChanged += numericUpDown2_ValueChanged;
            numericUpDown2.KeyDown += numericUpDownKeyDown;
            numericUpDown2.KeyPress += numericUpDownKeyPress;
            // 
            // numericUpDown3
            // 
            numericUpDown3.DecimalPlaces = 2;
            numericUpDown3.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numericUpDown3.Location = new System.Drawing.Point(72, 68);
            numericUpDown3.Maximum = new decimal(new int[] { 888, 0, 0, 131072 });
            numericUpDown3.Minimum = new decimal(new int[] { 8, 0, 0, 131072 });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new System.Drawing.Size(46, 23);
            numericUpDown3.TabIndex = 32;
            numericUpDown3.Value = new decimal(new int[] { 8, 0, 0, 131072 });
            numericUpDown3.ValueChanged += numericUpDown3_ValueChanged;
            numericUpDown3.KeyDown += numericUpDownKeyDown;
            numericUpDown3.KeyPress += numericUpDownKeyPress;
            // 
            // numericUpDownTargetAR
            // 
            numericUpDownTargetAR.DecimalPlaces = 1;
            numericUpDownTargetAR.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDownTargetAR.Location = new System.Drawing.Point(72, 97);
            numericUpDownTargetAR.Maximum = new decimal(new int[] { 11, 0, 0, 0 });
            numericUpDownTargetAR.Name = "numericUpDownTargetAR";
            numericUpDownTargetAR.Size = new System.Drawing.Size(46, 23);
            numericUpDownTargetAR.TabIndex = 20;
            numericUpDownTargetAR.KeyDown += numericUpDownAR_KeyDown;
            numericUpDownTargetAR.KeyPress += numericUpDownKeyPress;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = System.Drawing.Color.White;
            label2.Location = new System.Drawing.Point(7, 99);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(22, 15);
            label2.TabIndex = 33;
            label2.Text = "AR";
            // 
            // trackBarAR
            // 
            trackBarAR.BackColor = System.Drawing.Color.Black;
            trackBarAR.LargeChange = 100;
            trackBarAR.Location = new System.Drawing.Point(117, 98);
            trackBarAR.Maximum = 1100;
            trackBarAR.Minimum = 100;
            trackBarAR.Name = "trackBarAR";
            trackBarAR.Size = new System.Drawing.Size(116, 45);
            trackBarAR.SmallChange = 10;
            trackBarAR.TabIndex = 34;
            trackBarAR.TickFrequency = 10;
            trackBarAR.TickStyle = System.Windows.Forms.TickStyle.None;
            trackBarAR.Value = 1000;
            trackBarAR.Scroll += trackBarAR_Scroll;
            // 
            // labelConnected
            // 
            labelConnected.AutoSize = true;
            labelConnected.Font = new System.Drawing.Font("Segoe UI", 6.4F);
            labelConnected.ForeColor = System.Drawing.Color.SpringGreen;
            labelConnected.Location = new System.Drawing.Point(128, 96);
            labelConnected.Name = "labelConnected";
            labelConnected.Size = new System.Drawing.Size(81, 12);
            labelConnected.TabIndex = 35;
            labelConnected.Text = "Connected to TOsu";
            labelConnected.Visible = false;
            // 
            // labelTosuInfo
            // 
            labelTosuInfo.AutoSize = true;
            labelTosuInfo.Font = new System.Drawing.Font("Segoe UI", 6.4F);
            labelTosuInfo.ForeColor = System.Drawing.Color.Yellow;
            labelTosuInfo.Location = new System.Drawing.Point(128, 108);
            labelTosuInfo.Name = "labelTosuInfo";
            labelTosuInfo.Size = new System.Drawing.Size(37, 12);
            labelTosuInfo.TabIndex = 36;
            labelTosuInfo.Text = "In Menu";
            labelTosuInfo.Visible = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(383, 221);
            Controls.Add(labelTosuInfo);
            Controls.Add(labelConnected);
            Controls.Add(label2);
            Controls.Add(numericUpDown3);
            Controls.Add(numericUpDown2);
            Controls.Add(numericUpDown1);
            Controls.Add(panelGraph);
            Controls.Add(buttonReset);
            Controls.Add(comboBoxMonitors);
            Controls.Add(labelAR1);
            Controls.Add(comboBoxProfiles);
            Controls.Add(textBoxProfileName);
            Controls.Add(buttonLoadProfile);
            Controls.Add(buttonSaveProfile);
            Controls.Add(labelAR2);
            Controls.Add(labelGamma);
            Controls.Add(labelBrightness);
            Controls.Add(labelContrast);
            Controls.Add(buttonSaveAR1);
            Controls.Add(buttonSaveAR2);
            Controls.Add(numericUpDownTargetAR);
            Controls.Add(trackBarContrast);
            Controls.Add(trackBarBrightness);
            Controls.Add(trackBarGamma);
            Controls.Add(buttonSetAR3);
            Controls.Add(trackBarAR);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Text = "GammaTool Extra+";
            ((System.ComponentModel.ISupportInitialize)trackBarGamma).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarContrast).EndInit();
            panelGraph.ResumeLayout(false);
            panelGraph.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTargetAR).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarAR).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label labelTosuInfo;

        private System.Windows.Forms.Label labelConnected;

        private System.Windows.Forms.TrackBar trackBarAR;

        private System.Windows.Forms.Label label2;
        private Label label1;
        private NumericUpDown numericUpDown1;
        private NumericUpDown numericUpDown2;
        private NumericUpDown numericUpDown3;
    }
}