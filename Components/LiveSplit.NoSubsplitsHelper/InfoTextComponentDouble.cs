using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class InfoTextComponentDouble : IComponent
    {
        public string InformationName { get { return NameLabel.Text; } set { NameLabel.Text = value; } }
        public string InformationValue1 { get { return ValueLabel1.Text; } set { ValueLabel1.Text = value; } }
        public string InformationValue2 { get { return ValueLabel2.Text; } set { ValueLabel2.Text = value; } }
        public string DrawStyle { get; set; }
        public string SeparatorValue { get { return SeparatorLabel.Text; } set { SeparatorLabel.Text = value; } }

        public GraphicsCache Cache { get; set; }

        public ICollection<string> AlternateNameText { get { return NameLabel.AlternateText; } set { NameLabel.AlternateText = value; } }

        public SimpleLabel NameLabel { get; protected set; }
        public SimpleLabel ValueLabel1 { get; protected set; }
        public SimpleLabel ValueLabel2 { get; protected set; }
        public SimpleLabel SeparatorLabel { get; protected set; }
        private float ValueLabelSeparation => 10f;

        public string LongestString { get; set; }
        protected SimpleLabel NameMeasureLabel { get; set; }

        public float PaddingTop { get; set; }
        public float PaddingLeft => 7f;
        public float PaddingBottom { get; set; }
        public float PaddingRight => 7f;

        public bool DisplayTwoRows { get; set; }

        public float VerticalHeight { get; set; }

        public float MinimumWidth => 20;


        public float HorizontalWidth
            => Math.Max(NameMeasureLabel.ActualWidth, ValueLabel1.ActualWidth + ValueLabelSeparation + SeparatorLabel.ActualWidth + ValueLabelSeparation + ValueLabel2.ActualWidth) + 10;

        public float MinimumHeight { get; set; }

        public InfoTextComponentDouble(string informationName, string informationValue1, string informationValue2, string separator = "|")
        {
            Cache = new GraphicsCache();
            NameLabel = new SimpleLabel()
            {
                HorizontalAlignment = StringAlignment.Near,
                Text = informationName
            };
            ValueLabel1 = new SimpleLabel()
            {
                HorizontalAlignment = StringAlignment.Far,
                Text = informationValue1
            };
            ValueLabel2 = new SimpleLabel()
            {
                HorizontalAlignment = StringAlignment.Far,
                Text = informationValue2
            };
            SeparatorLabel = new SimpleLabel()
            {
                HorizontalAlignment = StringAlignment.Far,
                Text = separator
            };
            DrawStyle = "21";
            NameMeasureLabel = new SimpleLabel();
            MinimumHeight = 25;
            VerticalHeight = 31;
            LongestString = "";
        }

        public virtual void PrepareDraw(LiveSplitState state, LayoutMode mode)
        {
            NameMeasureLabel.Font = state.LayoutSettings.TextFont;
            ValueLabel1.Font = state.LayoutSettings.TextFont;
            SeparatorLabel.Font = state.LayoutSettings.TextFont;
            ValueLabel2.Font = state.LayoutSettings.TextFont;
            NameLabel.Font = state.LayoutSettings.TextFont;
            if (mode == LayoutMode.Vertical)
            {
                NameLabel.VerticalAlignment = StringAlignment.Center;
                ValueLabel1.VerticalAlignment = StringAlignment.Center;
                ValueLabel2.VerticalAlignment = StringAlignment.Center;
                SeparatorLabel.VerticalAlignment = StringAlignment.Center;
            }
            else
            {
                NameLabel.VerticalAlignment = StringAlignment.Near;
                ValueLabel1.VerticalAlignment = StringAlignment.Far;
                ValueLabel2.VerticalAlignment = StringAlignment.Far;
                SeparatorLabel.VerticalAlignment = StringAlignment.Far;
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            if (DisplayTwoRows)
            {
                VerticalHeight = 0.9f * (g.MeasureString("A", ValueLabel1.Font).Height + g.MeasureString("A", NameLabel.Font).Height);
                PaddingTop = PaddingBottom = 0;
                DrawTwoRows(g, state, width, VerticalHeight);
            }
            else
            {
                VerticalHeight = 31;
                NameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
                NameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
                ValueLabel1.ShadowColor = state.LayoutSettings.ShadowsColor;
                ValueLabel1.OutlineColor = state.LayoutSettings.TextOutlineColor;
                SeparatorLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
                SeparatorLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
                ValueLabel2.ShadowColor = state.LayoutSettings.ShadowsColor;
                ValueLabel2.OutlineColor = state.LayoutSettings.TextOutlineColor;

                var textHeight = 0.75f * Math.Max(g.MeasureString("A", ValueLabel1.Font).Height, g.MeasureString("A", NameLabel.Font).Height);
                PaddingTop = Math.Max(0, (VerticalHeight - textHeight) / 2f);
                PaddingBottom = PaddingTop;

                NameMeasureLabel.Text = LongestString;
                NameMeasureLabel.SetActualWidth(g);
                ValueLabel1.SetActualWidth(g);
                SeparatorLabel.SetActualWidth(g);
                ValueLabel2.SetActualWidth(g);

                string tempDraw = (string)DrawStyle.Clone();

                float actualWidth;
                if (tempDraw == "12" || tempDraw == "2")
                {
                    ValueLabel2.Width = ValueLabel2.IsMonospaced ? width - 12 : width - 10;
                    ValueLabel2.Height = VerticalHeight;
                    ValueLabel2.Y = 0;
                    ValueLabel2.X = 5;
                    actualWidth = ValueLabel2.ActualWidth + ValueLabelSeparation;
                }
                else
                {
                    ValueLabel1.Width = ValueLabel1.IsMonospaced ? width - 12 : width - 10;
                    ValueLabel1.Height = VerticalHeight;
                    ValueLabel1.Y = 0;
                    ValueLabel1.X = 5;
                    actualWidth = ValueLabel1.ActualWidth + ValueLabelSeparation;
                }

                if (tempDraw == "12" || tempDraw == "21")
                {
                    SeparatorLabel.Width = SeparatorLabel.IsMonospaced ? width - 12 : width - 10;
                    SeparatorLabel.Height = VerticalHeight;
                    SeparatorLabel.Y = 0;
                    SeparatorLabel.X = 5 - actualWidth;
                    actualWidth += SeparatorLabel.ActualWidth + ValueLabelSeparation;
                }
                
                if (tempDraw == "12")
                {
                    ValueLabel1.Width = ValueLabel1.IsMonospaced ? width - 12 : width - 10;
                    ValueLabel1.Height = VerticalHeight;
                    ValueLabel1.Y = 0;
                    ValueLabel1.X = 5 - actualWidth;
                    actualWidth += ValueLabel1.ActualWidth;
                }
                else if (tempDraw == "21")
                {
                    ValueLabel2.Width = ValueLabel1.IsMonospaced ? width - 12 : width - 10;
                    ValueLabel2.Height = VerticalHeight;
                    ValueLabel2.Y = 0;
                    ValueLabel2.X = 5 - actualWidth;
                    actualWidth += ValueLabel2.ActualWidth;
                }
                NameLabel.Width = width - actualWidth - 10;
                NameLabel.Height = VerticalHeight;
                NameLabel.X = 5;
                NameLabel.Y = 0;

                PrepareDraw(state, LayoutMode.Vertical);

                NameLabel.Draw(g);

                if (tempDraw == "1" || tempDraw == "12" || tempDraw == "21")
                {
                    ValueLabel1.Draw(g);
                }
                if (tempDraw == "2" || tempDraw == "12" || tempDraw == "21")
                {
                    ValueLabel2.Draw(g);
                }
                if (tempDraw == "12" || tempDraw == "21")
                {
                    SeparatorLabel.Draw(g);
                }
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawTwoRows(g, state, HorizontalWidth, height);
        }

        protected void DrawTwoRows(Graphics g, LiveSplitState state, float width, float height)
        {
            NameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            NameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            ValueLabel1.ShadowColor = state.LayoutSettings.ShadowsColor;
            ValueLabel1.OutlineColor = state.LayoutSettings.TextOutlineColor;
            SeparatorLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            SeparatorLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            ValueLabel2.ShadowColor = state.LayoutSettings.ShadowsColor;
            ValueLabel2.OutlineColor = state.LayoutSettings.TextOutlineColor;

            if (InformationName != null && LongestString != null && InformationName.Length > LongestString.Length)
            {
                LongestString = InformationName;
                NameMeasureLabel.Text = LongestString;
            }
            NameMeasureLabel.Text = LongestString;
            NameMeasureLabel.Font = state.LayoutSettings.TextFont;
            NameMeasureLabel.SetActualWidth(g);
            ValueLabel1.SetActualWidth(g);
            SeparatorLabel.SetActualWidth(g);
            ValueLabel2.SetActualWidth(g);

            MinimumHeight = 0.85f * (g.MeasureString("A", ValueLabel1.Font).Height + g.MeasureString("A", NameLabel.Font).Height);
            string tempDraw = (string)DrawStyle.Clone();

            float actualWidth;
            if (tempDraw == "12" || tempDraw == "2")
            {
                ValueLabel2.Width = ValueLabel2.IsMonospaced ? width - 12 : width - 10;
                ValueLabel2.Height = height;
                ValueLabel2.Y = 0;
                ValueLabel2.X = 5;
                actualWidth = ValueLabel2.ActualWidth + ValueLabelSeparation;
            }
            else
            {
                ValueLabel1.Width = ValueLabel1.IsMonospaced ? width - 12 : width - 10;
                ValueLabel1.Height = height;
                ValueLabel1.Y = 0;
                ValueLabel1.X = 5;
                actualWidth = ValueLabel1.ActualWidth + ValueLabelSeparation;
            }

            if (tempDraw == "12" || tempDraw == "21")
            {
                SeparatorLabel.Width = SeparatorLabel.IsMonospaced ? width - 12 : width - 10;
                SeparatorLabel.Height = height;
                SeparatorLabel.Y = 0;
                SeparatorLabel.X = 5 - actualWidth;
                actualWidth += SeparatorLabel.ActualWidth + ValueLabelSeparation;
            }

            if (tempDraw == "12")
            {
                ValueLabel1.Width = ValueLabel1.IsMonospaced ? width - 12 : width - 10;
                ValueLabel1.Height = height;
                ValueLabel1.Y = 0;
                ValueLabel1.X = 5 - actualWidth;
                actualWidth += ValueLabel1.ActualWidth;
            }
            else if (tempDraw == "21")
            {
                ValueLabel2.Width = ValueLabel1.IsMonospaced ? width - 12 : width - 10;
                ValueLabel2.Height = height;
                ValueLabel2.Y = 0;
                ValueLabel2.X = 5 - actualWidth;
                actualWidth += ValueLabel2.ActualWidth;
            }
            NameLabel.Width = width - 10;
            NameLabel.Height = height;
            NameLabel.X = 5;
            NameLabel.Y = 0;

            PrepareDraw(state, LayoutMode.Horizontal);

            NameLabel.Draw(g);

            if (tempDraw == "1" || tempDraw == "12" || tempDraw == "21")
            {
                ValueLabel1.Draw(g);
            }
            if (tempDraw == "2" || tempDraw == "12" || tempDraw == "21")
            {
                ValueLabel2.Draw(g);
            }
            if (tempDraw == "12" || tempDraw == "21")
            {
                SeparatorLabel.Draw(g);
            }
        }

        public string ComponentName
        {
            get { throw new NotSupportedException(); }
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            throw new NotImplementedException();
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            throw new NotImplementedException();
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            throw new NotImplementedException();
        }

        public string UpdateName
        {
            get { throw new NotSupportedException(); }
        }

        public string XMLURL
        {
            get { throw new NotSupportedException(); }
        }

        public string UpdateURL
        {
            get { throw new NotSupportedException(); }
        }

        public Version Version
        {
            get { throw new NotSupportedException(); }
        }

        public IDictionary<string, Action> ContextMenuControls => null;

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            Cache.Restart();
            Cache["NameText"] = InformationName;
            Cache["ValueText1"] = InformationValue1;
            Cache["SeparatorText"] = SeparatorValue;
            Cache["ValueText2"] = InformationValue2;
            Cache["NameColor"] = NameLabel.ForeColor.ToArgb();
            Cache["ValueColor1"] = ValueLabel1.ForeColor.ToArgb();
            Cache["SeparatorColor"] = SeparatorLabel.ForeColor.ToArgb();
            Cache["ValueColor2"] = ValueLabel2.ForeColor.ToArgb();
            Cache["DisplayTwoRows"] = DisplayTwoRows;
            Cache["DrawStyle"] = DrawStyle;

            if (invalidator != null && Cache.HasChanged)
            {
                invalidator.Invalidate(0, 0, width, height);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
