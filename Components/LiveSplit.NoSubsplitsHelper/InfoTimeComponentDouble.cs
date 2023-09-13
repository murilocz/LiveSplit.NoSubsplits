using LiveSplit.TimeFormatters;
using System;
using System.Drawing;

namespace LiveSplit.UI.Components
{
    public class InfoTimeComponentDouble : InfoTextComponentDouble
    {
        private TimeSpan? timeValue1;
        private TimeSpan? timeValue2;
        public TimeSpan? TimeValue1
        {
            get
            {
                return timeValue1;
            }
            set
            {
                timeValue1 = value;
                InformationValue1 = Formatter.Format(timeValue1);
            }
        }
        public TimeSpan? TimeValue2
        {
            get
            {
                return timeValue2;
            }
            set
            {
                timeValue2 = value;
                InformationValue2 = Formatter.Format(timeValue2);
            }
        }

        private ITimeFormatter formatter;
        public ITimeFormatter Formatter
        {
            get
            {
                return formatter;
            }
            set
            {
                if (value != null && value != formatter)
                {
                    InformationValue1 = value.Format(timeValue1);
                    InformationValue2 = value.Format(timeValue2);
                }
                formatter = value;
            }
        }

        public override void PrepareDraw(Model.LiveSplitState state, LayoutMode mode)
        {
            ValueLabel1.IsMonospaced = true;
            SeparatorLabel.IsMonospaced = true;
            ValueLabel2.IsMonospaced = true;
            ValueLabel1.Font = state.LayoutSettings.TimesFont;
            SeparatorLabel.Font = state.LayoutSettings.TimesFont;
            ValueLabel2.Font = state.LayoutSettings.TimesFont;
            NameMeasureLabel.Font = state.LayoutSettings.TextFont;
            NameLabel.Font = state.LayoutSettings.TextFont;
            if (mode == LayoutMode.Vertical)
            {
                NameLabel.VerticalAlignment = StringAlignment.Center;
                ValueLabel1.VerticalAlignment = StringAlignment.Center;
                SeparatorLabel.VerticalAlignment = StringAlignment.Center;
                ValueLabel2.VerticalAlignment = StringAlignment.Center;
            }
            else
            {
                NameLabel.VerticalAlignment = StringAlignment.Near;
                ValueLabel1.VerticalAlignment = StringAlignment.Far;
                SeparatorLabel.VerticalAlignment = StringAlignment.Far;
                ValueLabel2.VerticalAlignment = StringAlignment.Far;
            }
        }

        public InfoTimeComponentDouble(string informationName, TimeSpan? timeValue1, TimeSpan? timeValue2, ITimeFormatter formatter)
            : base(informationName, "", "")
        {
            Formatter = formatter;
            TimeValue1 = timeValue1;
            TimeValue2 = timeValue2;
        }
    }
}
