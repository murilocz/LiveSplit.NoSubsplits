using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.NoSubsplitsHelper;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.PreviousSegmentNoSubsplits.UI.Components
{
    public class PreviousSegmentNoSubsplits : IComponent
    {
        protected InfoTimeComponentDouble InternalComponent { get; set; }
        public PreviousSegmentNoSubsplitsSettings Settings { get; set; }

        protected DeltaTimeFormatter DeltaFormatter { get; set; }
        protected PossibleTimeSaveFormatter TimeSaveFormatter { get; set; }

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;

        private string previousNameText { get; set; }
        private string previousShortNameText { get; set; }

        public IDictionary<string, System.Action> ContextMenuControls => null;

        public PreviousSegmentNoSubsplits(LiveSplitState state)
        {
            state.ComparisonRenamed += state_ComparisonRenamed;
            DeltaFormatter = new DeltaTimeFormatter();
            TimeSaveFormatter = new PossibleTimeSaveFormatter();
            Settings = new PreviousSegmentNoSubsplitsSettings()
            {
                CurrentState = state
            };
            InternalComponent = new InfoTimeComponentDouble(null, null, null, DeltaFormatter);
        }

        void state_ComparisonRenamed(object sender, EventArgs e)
        {
            var args = (RenameEventArgs)e;
            if (Settings.Comparison == args.OldName)
            {
                Settings.Comparison = args.NewName;
                ((LiveSplitState)sender).Layout.HasChanged = true;
            }
        }

        private void PrepareDraw(LiveSplitState state)
        {
            InternalComponent.DisplayTwoRows = Settings.Display2Rows;
            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel1.HasShadow
                = InternalComponent.ValueLabel2.HasShadow
                = InternalComponent.SeparatorLabel.HasShadow
                = state.LayoutSettings.DropShadows;
            InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
             InternalComponent.SeparatorLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
        }

        private void DrawBackground(Graphics g, LiveSplitState state, float width, float height)
        {
            if (Settings.BackgroundColor.A > 0
                || Settings.BackgroundGradient != GradientType.Plain
                && Settings.BackgroundColor2.A > 0)
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            Settings.BackgroundGradient == GradientType.Horizontal
                            ? new PointF(width, 0)
                            : new PointF(0, height),
                            Settings.BackgroundColor,
                            Settings.BackgroundGradient == GradientType.Plain
                            ? Settings.BackgroundColor
                            : Settings.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            DrawBackground(g, state, width, VerticalHeight);
            PrepareDraw(state);
            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawBackground(g, state, HorizontalWidth, height);
            PrepareDraw(state);
            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public float VerticalHeight => InternalComponent.VerticalHeight;

        public float MinimumWidth => InternalComponent.MinimumWidth;

        public float HorizontalWidth => InternalComponent.HorizontalWidth;

        public float MinimumHeight => InternalComponent.MinimumHeight;
        public string ComponentName => _ComponentName();
        private string _ComponentName()
        {
            var comparisonName = (Settings.Comparison == "Current Comparison"
                ? ""
                : " (" + CompositeComparisons.GetShortComparisonName(Settings.Comparison) + ")");

            var infoName1 = "Previous ";

            if (Settings.DisplayStyle == "SingleGroup" || Settings.DisplayStyle == "Single")
            {
                infoName1 += Settings.DisplayNameSingle;
            }
            else if (Settings.DisplayStyle == "Group" || Settings.DisplayStyle == "GroupSingle")
            {
                infoName1 += Settings.DisplayNameGroup;
            }
            if (Settings.DisplayStyle == "SingleGroup" || Settings.DisplayStyle == "GroupSingle")
            {
                var sep = " " + InternalComponent.SeparatorValue + " ";
                infoName1 += sep;
            }
            if (Settings.DisplayStyle == "SingleGroup" || Settings.DisplayStyle == "GroupSingle")
            {
                infoName1 += "Previous ";
            }
            if (Settings.DisplayStyle == "GroupSingle")
            {
                infoName1 += Settings.DisplayNameSingle;
            }
            else if (Settings.DisplayStyle == "SingleGroup")
            {
                infoName1 += Settings.DisplayNameGroup;
            }

            return infoName1 + comparisonName;
        }
            

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public TimeSpan? GetPossibleTimeSave(LiveSplitState state, int splitIndex, string comparison)
        {
            var prevTime = TimeSpan.Zero;
            TimeSpan? bestSegments = state.Run[splitIndex].BestSegmentTime[state.CurrentTimingMethod];

            while (splitIndex > 0 && bestSegments != null)
            {
                var splitTime = state.Run[splitIndex - 1].Comparisons[comparison][state.CurrentTimingMethod];
                if (splitTime != null)
                {
                    prevTime = splitTime.Value;
                    break;
                }
                else
                {
                    splitIndex--;
                    bestSegments += state.Run[splitIndex].BestSegmentTime[state.CurrentTimingMethod];
                }
            }

            var time = state.Run[splitIndex].Comparisons[comparison][state.CurrentTimingMethod] - prevTime - bestSegments;

            if (time < TimeSpan.Zero)
                time = TimeSpan.Zero;

            return time;
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            var comparison = Settings.Comparison == "Current Comparison" ? state.CurrentComparison : Settings.Comparison;
            if (!state.Run.Comparisons.Contains(comparison))
                comparison = state.CurrentComparison;
            var comparisonName = CompositeComparisons.GetShortComparisonName(comparison);
            var componentNameComp = (Settings.Comparison == "Current Comparison" ? "" : " (" + comparisonName + ")");


            DeltaFormatter.Accuracy = Settings.DeltaAccuracy;
            DeltaFormatter.DropDecimals = Settings.DropDecimals;
            TimeSaveFormatter.Accuracy = Settings.TimeSaveAccuracy;

            TimeSpan? timeChange = null;
            TimeSpan? timeSave = null;
            var liveSegment = LiveSplitStateHelper.CheckLiveDelta(state, false, comparison, state.CurrentTimingMethod);
            if (state.CurrentPhase != TimerPhase.NotRunning)
            {
                if (liveSegment != null)
                {
                    timeChange = liveSegment;
                    timeSave = GetPossibleTimeSave(state, state.CurrentSplitIndex, comparison);
                }
                else if (state.CurrentSplitIndex > 0)
                {
                    timeChange = LiveSplitStateHelper.GetPreviousSegmentDelta(state, state.CurrentSplitIndex - 1, comparison, state.CurrentTimingMethod);
                    timeSave = GetPossibleTimeSave(state, state.CurrentSplitIndex - 1, comparison);
                }
                if (timeChange != null)
                {
                    if (liveSegment != null)
                        InternalComponent.ValueLabel1.ForeColor = LiveSplitStateHelper.GetSplitColor(state, timeChange, state.CurrentSplitIndex, false, false, comparison, state.CurrentTimingMethod).Value;
                    else
                        InternalComponent.ValueLabel1.ForeColor = LiveSplitStateHelper.GetSplitColor(state, timeChange.Value, state.CurrentSplitIndex - 1, false, true, comparison, state.CurrentTimingMethod).Value;
                }
                else
                {
                    var color = LiveSplitStateHelper.GetSplitColor(state, null, state.CurrentSplitIndex - 1, true, true, comparison, state.CurrentTimingMethod);
                    if (color == null)
                        color = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
                    InternalComponent.ValueLabel1.ForeColor = color.Value;
                }
            }
            else
            {
                InternalComponent.ValueLabel1.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            }

            InternalComponent.InformationValue1 = DeltaFormatter.Format(timeChange)
                + (Settings.ShowPossibleTimeSave ? " / " + TimeSaveFormatter.Format(timeSave) : "");

            state = NoSubsplitsHelper.NoSubsplitsHelper.fromSubsplitToSplit(state);
            TimeSpan? timeChangeGroup = null;
            TimeSpan? timeSaveGroup = null;
            var liveSegmentGroup = LiveSplitStateHelper.CheckLiveDelta(state, false, comparison, state.CurrentTimingMethod);
            if (state.CurrentPhase != TimerPhase.NotRunning)
            {
                if (liveSegmentGroup != null)
                {
                    timeChangeGroup = liveSegmentGroup;
                    timeSaveGroup = GetPossibleTimeSave(state, state.CurrentSplitIndex, comparison);
                }
                else if (state.CurrentSplitIndex > 0)
                {
                    timeChangeGroup = LiveSplitStateHelper.GetPreviousSegmentDelta(state, state.CurrentSplitIndex - 1, comparison, state.CurrentTimingMethod);
                    timeSaveGroup = GetPossibleTimeSave(state, state.CurrentSplitIndex - 1, comparison);
                }
                if (timeChangeGroup != null)
                {
                    if (liveSegmentGroup != null)
                        InternalComponent.ValueLabel2.ForeColor = LiveSplitStateHelper.GetSplitColor(state, timeChangeGroup, state.CurrentSplitIndex, false, false, comparison, state.CurrentTimingMethod).Value;
                    else
                        InternalComponent.ValueLabel2.ForeColor = LiveSplitStateHelper.GetSplitColor(state, timeChangeGroup.Value, state.CurrentSplitIndex - 1, false, true, comparison, state.CurrentTimingMethod).Value;
                }
                else
                {
                    var color = LiveSplitStateHelper.GetSplitColor(state, null, state.CurrentSplitIndex - 1, true, true, comparison, state.CurrentTimingMethod);
                    if (color == null)
                        color = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
                    InternalComponent.ValueLabel2.ForeColor = color.Value;
                }
            }
            else
            {
                InternalComponent.ValueLabel2.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            }
            InternalComponent.InformationValue2 = DeltaFormatter.Format(timeChangeGroup)
                + (Settings.ShowPossibleTimeSave ? " / " + TimeSaveFormatter.Format(timeSaveGroup) : "");

            var infoName1 = "";
            var infoName2 = "";
            var infoName3 = "";
            var infoName4 = "";

            if (((Settings.DisplayStyle == "Single" || Settings.DisplayStyle == "SingleGroup") && liveSegment      != null) ||
                ((Settings.DisplayStyle == "Group"  || Settings.DisplayStyle == "GroupSingle") && liveSegmentGroup != null))
            {
                infoName1 += "Live ";
                infoName2 += "Live ";
                infoName3 += "Live ";
            }
            else
            {
                infoName1 += "Previous ";
                infoName2 += "Prev. ";
                infoName3 += "Prev. ";
            }
            if (Settings.DisplayStyle == "SingleGroup" || Settings.DisplayStyle == "Single")
            {
                infoName1 += Settings.DisplayNameSingle;
                infoName2 += Settings.DisplayNameSingle;
                infoName3 += Settings.DisplayNameSingleShort;
            }
            else if (Settings.DisplayStyle == "Group" || Settings.DisplayStyle == "GroupSingle")
            {
                infoName1 += Settings.DisplayNameGroup;
                infoName2 += Settings.DisplayNameGroup;
                infoName3 += Settings.DisplayNameGroupShort;
            }
            if (Settings.DisplayStyle == "SingleGroup" || Settings.DisplayStyle == "GroupSingle")
            {
                var sep = " " + InternalComponent.SeparatorValue + " ";
                infoName1 += sep;
                infoName2 += sep;
                infoName3 += sep;
            }
            if ((Settings.DisplayStyle == "SingleGroup" && liveSegmentGroup != null) ||
                (Settings.DisplayStyle == "GroupSingle" && liveSegment      != null))
            {
                infoName1 += "Live ";
                infoName2 += "Live ";
                infoName3 += "Live ";
            }
            else if (Settings.DisplayStyle == "SingleGroup" || Settings.DisplayStyle == "GroupSingle")
            {
                infoName1 += "Previous ";
                infoName2 += "Prev. ";
                infoName3 += "Prev. ";
            }
            if (Settings.DisplayStyle == "GroupSingle")
            {
                infoName1 += Settings.DisplayNameSingle;
                infoName2 += Settings.DisplayNameSingle;
                infoName3 += Settings.DisplayNameSingleShort;
            }
            else if (Settings.DisplayStyle == "SingleGroup")
            {
                infoName1 += Settings.DisplayNameGroup;
                infoName2 += Settings.DisplayNameGroup;
                infoName3 += Settings.DisplayNameGroupShort;
            }

            infoName4 = infoName3;
            infoName3 = infoName3 + componentNameComp;
            infoName2 = infoName2 + componentNameComp;
            infoName1 = infoName1 + componentNameComp;
            InternalComponent.InformationName = infoName1;
            InternalComponent.LongestString = infoName1;

            if (InternalComponent.InformationName != previousNameText || infoName4 != previousShortNameText)
            {
                InternalComponent.AlternateNameText.Clear();
                InternalComponent.AlternateNameText.Add(infoName2);
                InternalComponent.AlternateNameText.Add(infoName3);
                InternalComponent.AlternateNameText.Add(infoName4);
                previousNameText = InternalComponent.InformationName;
                previousShortNameText = infoName4;
            }

            if (Settings.DisplayStyle == "Single")
                InternalComponent.DrawStyle = "1";
            else if (Settings.DisplayStyle == "Group")
                InternalComponent.DrawStyle = "2";
            else if (Settings.DisplayStyle == "SingleGroup")
                InternalComponent.DrawStyle = "12";
            else if (Settings.DisplayStyle == "GroupSingle")
                InternalComponent.DrawStyle = "21";
            else
            {
                InternalComponent.DrawStyle = null;
            }

            InternalComponent.Update(invalidator, state, width, height, mode);
        }

        public void Dispose()
        {
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}
