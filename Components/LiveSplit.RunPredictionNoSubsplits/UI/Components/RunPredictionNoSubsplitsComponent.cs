using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class RunPredictionNoSubsplits : IComponent
    {
        protected InfoTimeComponentDouble InternalComponent { get; set; }
        public RunPredictionNoSubsplitsSettings Settings { get; set; }
        private RunPredictionFormatter Formatter { get; set; }
        private string PreviousInformationName { get; set; }
        private string PreviousInformationNameShort { get; set; }

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;

        public IDictionary<string, Action> ContextMenuControls => null;

        public RunPredictionNoSubsplits(LiveSplitState state)
        {
            Settings = new RunPredictionNoSubsplitsSettings()
            {
                CurrentState = state
            };
            Formatter = new RunPredictionFormatter(Settings.Accuracy);
            InternalComponent = new InfoTimeComponentDouble(null, null, null, Formatter);
            state.ComparisonRenamed += state_ComparisonRenamed;
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

            Formatter.Accuracy = Settings.Accuracy;

            InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            InternalComponent.SeparatorLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel1.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel2.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;
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
            string result = GetDisplayedName(Settings.Comparison);
            result += " (";
            if (Settings.DisplayStyle == "SingleGroup" || Settings.DisplayStyle == "Single")
            {
                result += Settings.DisplayNameSingle;
            }
            else if (Settings.DisplayStyle == "Group" || Settings.DisplayStyle == "GroupSingle")
            {
                result += Settings.DisplayNameGroup;
            }
            if (Settings.DisplayStyle == "SingleGroup" || Settings.DisplayStyle == "GroupSingle")
            {
                var sep = " " + InternalComponent.SeparatorValue + " ";
                result += sep;
            }
            if (Settings.DisplayStyle == "GroupSingle")
            {
                result += Settings.DisplayNameSingle;
            }
            else if (Settings.DisplayStyle == "SingleGroup")
            {
                result += Settings.DisplayNameGroup;
            }
            result += ")";
            return result;
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

        protected string GetDisplayedName(string comparison)
        {
            switch (comparison)
            {
                case "Current Comparison":
                    return "Current Pace";
                case Run.PersonalBestComparisonName:
                    return "Current Pace";
                case BestSegmentsComparisonGenerator.ComparisonName:
                    return "Best Possible Time";
                case WorstSegmentsComparisonGenerator.ComparisonName:
                    return "Worst Possible Time";
                case AverageSegmentsComparisonGenerator.ComparisonName:
                    return "Predicted Time";
                default:
                    return "Current Pace (" + CompositeComparisons.GetShortComparisonName(comparison) + ")";
            }
        }

        protected void SetAlternateText(string comparison, string ending1, string ending2)
        {
            switch (comparison)
            {
                case "Current Comparison":
                    InternalComponent.AlternateNameText = new[]
                    {
                        "Cur. Pace" + ending1,
                        "Cur. Pace" + ending2,
                        "Pace" + ending1,
                        "Pace" + ending2
                    };
                    break;
                case Run.PersonalBestComparisonName:
                    InternalComponent.AlternateNameText = new[]
                    {
                        "Cur. Pace" + ending1,
                        "Cur. Pace" + ending2,
                        "Pace" + ending1,
                        "Pace" + ending2
                    };
                    break;
                case BestSegmentsComparisonGenerator.ComparisonName:
                    InternalComponent.AlternateNameText = new[]
                    {
                        "Best Poss. Time" + ending1,
                        "Best Poss. Time" + ending2,
                        "Best Time" + ending1,
                        "Best Time" + ending2,
                        "BPT" + ending1,
                        "BPT" + ending2
                    };
                    break;
                case WorstSegmentsComparisonGenerator.ComparisonName:
                    InternalComponent.AlternateNameText = new[]
                    {
                        "Worst Poss. Time" + ending1,
                        "Worst Poss. Time" + ending2,
                        "Worst Time" + ending1,
                        "Worst Time" + ending2
                    };
                    break;
                case AverageSegmentsComparisonGenerator.ComparisonName:
                    InternalComponent.AlternateNameText = new[]
                    {
                        "Pred. Time" + ending1,
                        "Pred. Time" + ending2
                    };
                    break;
                default:
                    InternalComponent.AlternateNameText = new[]
                    {
                        "Current Pace" + ending1,
                        "Current Pace" + ending2,
                        "Cur. Pace" + ending1,
                        "Cur. Pace" + ending2,
                        "Pace" + ending1,
                        "Pace" + ending2
                    };
                    break;
            }
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            var comparison = Settings.Comparison == "Current Comparison" ? state.CurrentComparison : Settings.Comparison;
            if (!state.Run.Comparisons.Contains(comparison))
                comparison = state.CurrentComparison;

            var infoName = GetDisplayedName(comparison);

            if (infoName.StartsWith("Current Pace") && state.CurrentPhase == TimerPhase.NotRunning)
            {
                InternalComponent.TimeValue1 = null;
            }
            else if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                TimeSpan? delta = LiveSplitStateHelper.GetLastDelta(state, state.CurrentSplitIndex, comparison, state.CurrentTimingMethod) ?? TimeSpan.Zero;
                var liveDelta = state.CurrentTime[state.CurrentTimingMethod] - state.CurrentSplit.Comparisons[comparison][state.CurrentTimingMethod];
                if (liveDelta > delta)
                    delta = liveDelta;
                InternalComponent.TimeValue1 = delta + state.Run.Last().Comparisons[comparison][state.CurrentTimingMethod];
            }
            else if (state.CurrentPhase == TimerPhase.Ended)
            {
                InternalComponent.TimeValue1 = state.Run.Last().SplitTime[state.CurrentTimingMethod];
            }
            else
            {
                InternalComponent.TimeValue1 = state.Run.Last().Comparisons[comparison][state.CurrentTimingMethod];
            }

            state = NoSubsplitsHelper.NoSubsplitsHelper.fromSubsplitToSplit(state);

            if (infoName.StartsWith("Current Pace") && state.CurrentPhase == TimerPhase.NotRunning)
            {
                InternalComponent.TimeValue2 = null;
            }
            else if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                TimeSpan? delta = LiveSplitStateHelper.GetLastDelta(state, state.CurrentSplitIndex, comparison, state.CurrentTimingMethod) ?? TimeSpan.Zero;
                var liveDelta = state.CurrentTime[state.CurrentTimingMethod] - state.CurrentSplit.Comparisons[comparison][state.CurrentTimingMethod];
                if (liveDelta > delta)
                    delta = liveDelta;
                InternalComponent.TimeValue2 = delta + state.Run.Last().Comparisons[comparison][state.CurrentTimingMethod];
            }
            else if (state.CurrentPhase == TimerPhase.Ended)
            {
                InternalComponent.TimeValue2 = state.Run.Last().SplitTime[state.CurrentTimingMethod];
            }
            else
            {
                InternalComponent.TimeValue2 = state.Run.Last().Comparisons[comparison][state.CurrentTimingMethod];
            }

            var ending1 = "";
            var ending2 = "";
            ending1 += " (";
            ending2 += " (";
            if (Settings.DisplayStyle == "SingleGroup" || Settings.DisplayStyle == "Single")
            {
                ending1 += Settings.DisplayNameSingle;
                ending2 += Settings.DisplayNameSingleShort;
            }
            else if (Settings.DisplayStyle == "Group" || Settings.DisplayStyle == "GroupSingle")
            {
                ending1 += Settings.DisplayNameGroup;
                ending2 += Settings.DisplayNameGroupShort;
            }
            if (Settings.DisplayStyle == "SingleGroup" || Settings.DisplayStyle == "GroupSingle")
            {
                var sep = " " + InternalComponent.SeparatorValue + " ";
                ending1 += sep;
                ending2 += sep;
            }
            if (Settings.DisplayStyle == "GroupSingle")
            {
                ending1 += Settings.DisplayNameSingle;
                ending2 += Settings.DisplayNameSingleShort;
            }
            else if (Settings.DisplayStyle == "SingleGroup")
            {
                ending1 += Settings.DisplayNameGroup;
                ending2 += Settings.DisplayNameGroupShort;
            }
            ending1 += ")";
            ending2 += ")";
            var infoName1 = infoName + ending1;
            var infoName2 = infoName + ending2;
            InternalComponent.InformationName = InternalComponent.LongestString = infoName1;
            
            if (infoName1 != PreviousInformationName || infoName2 != PreviousInformationNameShort)
            {
                SetAlternateText(comparison, ending1, ending2);
                PreviousInformationName = infoName1;
                PreviousInformationNameShort = infoName2;
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
