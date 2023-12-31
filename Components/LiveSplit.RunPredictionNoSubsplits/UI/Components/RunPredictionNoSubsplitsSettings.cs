﻿using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class RunPredictionNoSubsplitsSettings : UserControl
    {
        public Color TextColor { get; set; }
        public bool OverrideTextColor { get; set; }
        public Color TimeColor { get; set; }
        public bool OverrideTimeColor { get; set; }
        public TimeAccuracy Accuracy { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public GradientType BackgroundGradient { get; set; }
        public string GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public string Comparison { get; set; }
        public LiveSplitState CurrentState { get; set; }
        public bool Display2Rows { get; set; }

        public LayoutMode Mode { get; set; }
        public string DisplayNameGroup { get; set; }
        public string DisplayNameGroupShort { get; set; }
        public string DisplayNameSingle { get; set; }
        public string DisplayNameSingleShort { get; set; }
        public string DisplayStyle { get; set; }

        public RunPredictionNoSubsplitsSettings()
        {
            InitializeComponent();

            TextColor = Color.FromArgb(255, 255, 255);
            OverrideTextColor = false;
            TimeColor = Color.FromArgb(255, 255, 255);
            OverrideTimeColor = false;
            Accuracy = TimeAccuracy.Seconds;
            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = GradientType.Plain;
            Comparison = "Current Comparison";
            DisplayNameGroup = "Chapter";
            DisplayNameGroupShort = "Ch.";
            DisplayNameSingle = "Checkpoint";
            DisplayNameSingleShort = "CP";
            DisplayStyle = "Group";
            Display2Rows = false;

            chkOverrideTextColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTextColor.DataBindings.Add("BackColor", this, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTimeColor.DataBindings.Add("Checked", this, "OverrideTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTimeColor.DataBindings.Add("BackColor", this, "TimeColor", false, DataSourceUpdateMode.OnPropertyChanged);

            cmbGradientType.SelectedIndexChanged += cmbGradientType_SelectedIndexChanged;
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbComparison.SelectedIndexChanged += cmbComparison_SelectedIndexChanged;
            cmbComparison.DataBindings.Add("SelectedItem", this, "Comparison", false, DataSourceUpdateMode.OnPropertyChanged);

            rdoSeconds.CheckedChanged += rdoSeconds_CheckedChanged;
            rdoHundredths.CheckedChanged += rdoHundredths_CheckedChanged;

            chkOverrideTextColor.CheckedChanged += chkOverrideTextColor_CheckedChanged;
            chkOverrideTimeColor.CheckedChanged += chkOverrideTimeColor_CheckedChanged;

            txtBoxGroup.DataBindings.Add("Text", this, "DisplayNameGroup", false, DataSourceUpdateMode.OnPropertyChanged);
            txtBoxGroupShort.DataBindings.Add("Text", this, "DisplayNameGroupShort", false, DataSourceUpdateMode.OnPropertyChanged);
            txtBoxSingle.DataBindings.Add("Text", this, "DisplayNameSingle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtBoxSingleShort.DataBindings.Add("Text", this, "DisplayNameSingleShort", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        void chkOverrideTimeColor_CheckedChanged(object sender, EventArgs e)
        {
            label2.Enabled = btnTimeColor.Enabled = chkOverrideTimeColor.Checked;
        }

        void chkOverrideTextColor_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = btnTextColor.Enabled = chkOverrideTextColor.Checked;
        }
        void cmbComparison_SelectedIndexChanged(object sender, EventArgs e)
        {
            Comparison = cmbComparison.SelectedItem.ToString();
        }

        void RunPredictionSettings_Load(object sender, EventArgs e)
        {
            chkOverrideTextColor_CheckedChanged(null, null);
            chkOverrideTimeColor_CheckedChanged(null, null);
            cmbComparison.Items.Clear();
            cmbComparison.Items.Add("Current Comparison");
            cmbComparison.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x != BestSplitTimesComparisonGenerator.ComparisonName && x != NoneComparisonGenerator.ComparisonName).ToArray());
            if (!cmbComparison.Items.Contains(Comparison))
                cmbComparison.Items.Add(Comparison);

            txtBoxGroup.Text = DisplayNameGroup;
            txtBoxGroupShort.Text = DisplayNameGroupShort;
            txtBoxSingle.Text = DisplayNameSingle;
            txtBoxSingleShort.Text = DisplayNameSingleShort;

            rdoSeconds.Checked = Accuracy == TimeAccuracy.Seconds;
            rdoTenths.Checked = Accuracy == TimeAccuracy.Tenths;
            rdoHundredths.Checked = Accuracy == TimeAccuracy.Hundredths;
            if (Mode == LayoutMode.Horizontal)
            {
                chkTwoRows.Enabled = false;
                chkTwoRows.DataBindings.Clear();
                chkTwoRows.Checked = true;
            }
            else
            {
                chkTwoRows.Enabled = true;
                chkTwoRows.DataBindings.Clear();
                chkTwoRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
            }

            rdoSingle.Checked = DisplayStyle == "Single";
            rdoGroup.Checked = DisplayStyle == "Group";
            rdoSingleGroup.Checked = DisplayStyle == "SingleGroup";
            rdoGroupSingle.Checked = DisplayStyle == "GroupSingle";
        }
        void UpdateDisplayStyle()
        {
            if (rdoSingle.Checked)
                DisplayStyle = "Single";
            else if (rdoGroup.Checked)
                DisplayStyle = "Group";
            else if (rdoSingleGroup.Checked)
                DisplayStyle = "SingleGroup";
            else
                DisplayStyle = "GroupSingle";
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        void rdoHundredths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        void rdoSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        void UpdateAccuracy()
        {
            if (rdoSeconds.Checked)
                Accuracy = TimeAccuracy.Seconds;
            else if (rdoTenths.Checked)
                Accuracy = TimeAccuracy.Tenths;
            else
                Accuracy = TimeAccuracy.Hundredths;
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            TextColor = SettingsHelper.ParseColor(element["TextColor"]);
            OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"]);
            TimeColor = SettingsHelper.ParseColor(element["TimeColor"]);
            OverrideTimeColor = SettingsHelper.ParseBool(element["OverrideTimeColor"]);
            Accuracy = SettingsHelper.ParseEnum<TimeAccuracy>(element["Accuracy"]);
            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"]);
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"]);
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"]);
            Comparison = SettingsHelper.ParseString(element["Comparison"]);
            DisplayStyle = SettingsHelper.ParseString(element["DisplayStyle"]);
            DisplayNameGroup = SettingsHelper.ParseString(element["DisplayNameGroup"]);
            DisplayNameGroupShort = SettingsHelper.ParseString(element["DisplayNameGroupShort"]);
            DisplayNameSingle = SettingsHelper.ParseString(element["DisplayNameSingle"]);
            DisplayNameSingleShort = SettingsHelper.ParseString(element["DisplayNameSingleShort"]);
            Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.4") ^
            SettingsHelper.CreateSetting(document, parent, "TextColor", TextColor) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
            SettingsHelper.CreateSetting(document, parent, "TimeColor", TimeColor) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTimeColor", OverrideTimeColor) ^
            SettingsHelper.CreateSetting(document, parent, "Accuracy", Accuracy) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
            SettingsHelper.CreateSetting(document, parent, "Comparison", Comparison) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayStyle", DisplayStyle) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayNameGroup", DisplayNameGroup) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayNameGroupShort", DisplayNameGroupShort) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayNameSingle", DisplayNameSingle) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayNameSingleShort", DisplayNameSingleShort) ^
            SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows);
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void txtBoxGroup_TextChanged(object sender, EventArgs e)
        {
            DisplayNameGroup = txtBoxGroup.Text;
        }
        private void txtBoxGroupShort_TextChanged(object sender, EventArgs e)
        {
            DisplayNameGroupShort = txtBoxGroupShort.Text;
        }
        private void txtBoxSingle_TextChanged(object sender, EventArgs e)
        {
            DisplayNameSingle = txtBoxSingle.Text;
        }
        private void txtBoxSingleShort_TextChanged(object sender, EventArgs e)
        {
            DisplayNameSingleShort = txtBoxSingleShort.Text;
        }

        private void rdoSingle_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisplayStyle();
        }

        private void rdoGroup_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisplayStyle();
        }

        private void rdoSingleGroup_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisplayStyle();
        }
    }
}
