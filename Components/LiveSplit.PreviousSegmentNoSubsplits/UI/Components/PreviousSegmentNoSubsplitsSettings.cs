using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using System;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.PreviousSegmentNoSubsplits.UI.Components
{
    public partial class PreviousSegmentNoSubsplitsSettings : UserControl
    {
        public Color TextColor { get; set; }
        public bool OverrideTextColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public GradientType BackgroundGradient { get; set; }
        public string GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public TimeAccuracy DeltaAccuracy { get; set; }
        public bool DropDecimals { get; set; }
        public bool Display2Rows { get; set; }
        public bool ShowPossibleTimeSave { get; set; }
        public TimeAccuracy TimeSaveAccuracy { get; set; }

        public string Comparison { get; set; }
        public LiveSplitState CurrentState { get; set; }

        public LayoutMode Mode { get; set; }
        public string DisplayNameGroup { get; set; }
        public string DisplayNameGroupShort { get; set; }
        public string DisplayNameSingle { get; set; }
        public string DisplayNameSingleShort { get; set; }
        public string DisplayStyle { get; set; }

        public PreviousSegmentNoSubsplitsSettings()
        {
            InitializeComponent();

            TextColor = Color.FromArgb(255, 255, 255);
            OverrideTextColor = false;
            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = GradientType.Plain;
            DeltaAccuracy = TimeAccuracy.Tenths;
            TimeSaveAccuracy = TimeAccuracy.Tenths;
            DropDecimals = true;
            Comparison = "Current Comparison";
            DisplayNameGroup = "Chapter";
            DisplayNameGroupShort = "Ch.";
            DisplayNameSingle = "Checkpoint";
            DisplayNameSingleShort = "CP";
            DisplayStyle = "Group";
            Display2Rows = false;
            ShowPossibleTimeSave = false;

            btnTextColor.DataBindings.Add("BackColor", this, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverride.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDropDecimals.DataBindings.Add("Checked", this, "DropDecimals", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbComparison.DataBindings.Add("SelectedItem", this, "Comparison", false, DataSourceUpdateMode.OnPropertyChanged);
            chkPossibleTimeSave.DataBindings.Add("Checked", this, "ShowPossibleTimeSave", false, DataSourceUpdateMode.OnPropertyChanged);
            txtBoxGroup.DataBindings.Add("Text", this, "DisplayNameGroup", false, DataSourceUpdateMode.OnPropertyChanged);
            txtBoxGroupShort.DataBindings.Add("Text", this, "DisplayNameGroupShort", false, DataSourceUpdateMode.OnPropertyChanged);
            txtBoxSingle.DataBindings.Add("Text", this, "DisplayNameSingle", false, DataSourceUpdateMode.OnPropertyChanged);
            txtBoxSingleShort.DataBindings.Add("Text", this, "DisplayNameSingleShort", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        void chkOverride_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = btnTextColor.Enabled = chkOverride.Checked;
        }

        void cmbComparison_SelectedIndexChanged(object sender, EventArgs e)
        {
            Comparison = cmbComparison.SelectedItem.ToString();
        }

        void rdoDeltaTenths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDeltaAccuracy();
        }

        void rdoDeltaSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDeltaAccuracy();
        }

        void PreviousSegmentSettings_Load(object sender, EventArgs e)
        {
            chkOverride_CheckedChanged(null, null);
            chkPossibleTimeSave_CheckedChanged(null, null);
            cmbComparison.Items.Clear();
            cmbComparison.Items.Add("Current Comparison");
            
            txtBoxGroup.Text = DisplayNameGroup;
            txtBoxGroupShort.Text = DisplayNameGroupShort;
            txtBoxSingle.Text = DisplayNameSingle;
            txtBoxSingleShort.Text = DisplayNameSingleShort;

            cmbComparison.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x != BestSplitTimesComparisonGenerator.ComparisonName && x != NoneComparisonGenerator.ComparisonName).ToArray());
            if (!cmbComparison.Items.Contains(Comparison))
                cmbComparison.Items.Add(Comparison);
            rdoDeltaHundredths.Checked = DeltaAccuracy == TimeAccuracy.Hundredths;
            rdoDeltaTenths.Checked = DeltaAccuracy == TimeAccuracy.Tenths;
            rdoDeltaSeconds.Checked = DeltaAccuracy == TimeAccuracy.Seconds;
            rdoTimeSaveHundredths.Checked = TimeSaveAccuracy == TimeAccuracy.Hundredths;
            rdoTimeSaveTenths.Checked = TimeSaveAccuracy == TimeAccuracy.Tenths;
            rdoTimeSaveSeconds.Checked = TimeSaveAccuracy == TimeAccuracy.Seconds;

            rdoSingle.Checked = DisplayStyle == "Single";
            rdoGroup.Checked = DisplayStyle == "Group";
            rdoSingleGroup.Checked = DisplayStyle == "SingleGroup";
            rdoGroupSingle.Checked = DisplayStyle == "GroupSingle";

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
        }

        void UpdateDeltaAccuracy()
        {
            if (rdoDeltaSeconds.Checked)
                DeltaAccuracy = TimeAccuracy.Seconds;
            else if (rdoDeltaTenths.Checked)
                DeltaAccuracy = TimeAccuracy.Tenths;
            else
                DeltaAccuracy = TimeAccuracy.Hundredths;
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

        void UpdateTimeSaveAccuracy()
        {
            if (rdoTimeSaveSeconds.Checked)
                TimeSaveAccuracy = TimeAccuracy.Seconds;
            else if (rdoTimeSaveTenths.Checked)
                TimeSaveAccuracy = TimeAccuracy.Tenths;
            else
                TimeSaveAccuracy = TimeAccuracy.Hundredths;
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            TextColor = SettingsHelper.ParseColor(element["TextColor"]);
            OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"]);
            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"]);
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"]);
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"]);
            DeltaAccuracy = SettingsHelper.ParseEnum<TimeAccuracy>(element["DeltaAccuracy"]);
            DropDecimals = SettingsHelper.ParseBool(element["DropDecimals"]);
            Comparison = SettingsHelper.ParseString(element["Comparison"]);
            DisplayStyle = SettingsHelper.ParseString(element["DisplayStyle"]);
            DisplayNameGroup = SettingsHelper.ParseString(element["DisplayNameGroup"]);
            DisplayNameGroupShort = SettingsHelper.ParseString(element["DisplayNameGroupShort"]);
            DisplayNameSingle = SettingsHelper.ParseString(element["DisplayNameSingle"]);
            DisplayNameSingleShort = SettingsHelper.ParseString(element["DisplayNameSingleShort"]);
            Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
            ShowPossibleTimeSave = SettingsHelper.ParseBool(element["ShowPossibleTimeSave"], false);
            TimeSaveAccuracy = SettingsHelper.ParseEnum<TimeAccuracy>(element["TimeSaveAccuracy"], TimeAccuracy.Tenths);
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
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.6") ^
            SettingsHelper.CreateSetting(document, parent, "TextColor", TextColor) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
            SettingsHelper.CreateSetting(document, parent, "DeltaAccuracy", DeltaAccuracy) ^
            SettingsHelper.CreateSetting(document, parent, "DropDecimals", DropDecimals) ^
            SettingsHelper.CreateSetting(document, parent, "Comparison", Comparison) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayStyle", DisplayStyle) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayNameGroup", DisplayNameGroup) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayNameGroupShort", DisplayNameGroupShort) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayNameSingle", DisplayNameSingle) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayNameSingleShort", DisplayNameSingleShort) ^
            SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows) ^
            SettingsHelper.CreateSetting(document, parent, "ShowPossibleTimeSave", ShowPossibleTimeSave) ^
            SettingsHelper.CreateSetting(document, parent, "TimeSaveAccuracy", TimeSaveAccuracy);
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void rdoTimeSaveSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTimeSaveAccuracy();
        }

        private void rdoTimeSaveHundredths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTimeSaveAccuracy();
        }

        private void chkPossibleTimeSave_CheckedChanged(object sender, EventArgs e)
        {
            rdoTimeSaveSeconds.Enabled = rdoTimeSaveTenths.Enabled = rdoTimeSaveHundredths.Enabled = boxTimeSaveAccuracy.Enabled = chkPossibleTimeSave.Checked;
        }

        private void txtBoxNameGroup_TextChanged(object sender, EventArgs e)
        {
            DisplayNameGroup = txtBoxGroup.Text;
        }
        private void txtBoxNameGroupShort_TextChanged(object sender, EventArgs e)
        {
            DisplayNameGroupShort = txtBoxGroupShort.Text;
        }
        private void txtBoxNameSingle_TextChanged(object sender, EventArgs e)
        {
            DisplayNameSingle = txtBoxSingle.Text;
        }
        private void txtBoxNameSingleShort_TextChanged(object sender, EventArgs e)
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
