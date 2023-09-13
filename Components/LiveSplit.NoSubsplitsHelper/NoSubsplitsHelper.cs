using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveSplit.NoSubsplitsHelper
{
    public class NoSubsplitsHelper
    {
        public static string UpdateURL = "https://raw.githubusercontent.com/murilocz/LiveSplit.NoSubsplits/master/";
        public static string XMLURL = "https://raw.githubusercontent.com/murilocz/LiveSplit.NoSubsplits/master/Components/update.LiveSplit.NoSubsplits.xml";
        public static string Version = "1.0.0";

        private static Regex SubsplitRegex = new Regex(@"^{(.+)}\s*(.+)$", RegexOptions.Compiled);
        private static bool changed = false;
        private static IRun updateNoSubsplit(IRun Run, IRun prevResult, out bool changed, bool sameSplit = false) // changed is if the segment history changed
        {
            changed = prevResult == null;
            if (!changed)
                if (Run.AttemptHistory.Count != prevResult.AttemptHistory.Count || Run.ComparisonGenerators.Count != prevResult.ComparisonGenerators.Count)
                    changed = true;

            if (!changed && sameSplit)
                return prevResult;
            IRun result = (IRun)Run.Clone();
            result.Clear();
            var splitHistory = new List<SegmentHistory>();
            if (changed)
            {
                var allHistory = new List<SegmentHistory>();
                foreach (var segment in Run)
                    allHistory.Add(new SegmentHistory());
                foreach (TimingMethod method in Enum.GetValues(typeof(TimingMethod)))
                    foreach (var attempt in Run.AttemptHistory)
                    {
                        var ind = attempt.Index;
                        var ignoreNextHistory = false;
                        foreach (var segment in Run)
                        {
                            Time history;
                            if (segment.SegmentHistory.TryGetValue(ind, out history))
                            {
                                if (history[method] == null)
                                {
                                    ignoreNextHistory = true;
                                }
                                else if (!ignoreNextHistory)
                                {
                                    Time t = allHistory[Run.IndexOf(segment)].ContainsKey(ind) ? allHistory[Run.IndexOf(segment)][ind] : new Time();
                                    t[method] = history[method].Value;
                                    allHistory[Run.IndexOf(segment)][ind] = t;
                                }
                                else
                                {
                                    ignoreNextHistory = false;
                                }
                            }
                            else
                            {
                                ignoreNextHistory = false;
                            }
                        }
                    }

                int previdx = -1;
                for (var idx = 0; idx < Run.Count; idx++)
                {
                    var isSubsplit = Run[idx].Name.StartsWith("-") && idx != Run.Count - 1;
                    if (isSubsplit)
                        continue;
                    var curHistory = new SegmentHistory();
                    foreach (int i in allHistory[idx].Keys)
                    {
                        Time? aggregate = Time.Zero;
                        for (var idxInner = previdx + 1; idxInner <= idx; idxInner++)
                        {
                            if (allHistory[idxInner].ContainsKey(i))
                                aggregate += allHistory[idxInner][i];
                            else
                                aggregate = null;
                        }
                        if (aggregate.HasValue)
                            curHistory[i] = aggregate.Value;
                    }
                    splitHistory.Add(curHistory);
                    previdx = idx;
                }
            }

            List<int> noSubsplitIndexes = new List<int>();
            for (var idx = 0; idx < Run.Count; idx++)
            {
                bool isSubsplit = Run[idx].Name.StartsWith("-") && idx != Run.Count - 1;
                if (!isSubsplit)
                    noSubsplitIndexes.Add(idx);
            }
            var totalBestTime = new Dictionary<TimingMethod, TimeSpan>();
            foreach (TimingMethod method in Enum.GetValues(typeof(TimingMethod)))
                totalBestTime[method] = TimeSpan.Zero;
            for (var idx2 = 0; idx2 < noSubsplitIndexes.Count; idx2++)
            {
                var startidx = idx2 > 0 ? noSubsplitIndexes[idx2 - 1] + 1 : 0;
                var lastidx = noSubsplitIndexes[idx2];

                var curList = changed ? splitHistory[idx2] : prevResult[idx2].SegmentHistory;
                if (curList.Count == 0)
                    totalBestTime = null;
                foreach (TimingMethod method in Enum.GetValues(typeof(TimingMethod)))
                    if (totalBestTime != null)
                    {
                        TimeSpan? temp = curList.Where(x => x.Value[method] != null).Min(x => x.Value[method]);
                        if (temp.HasValue)
                            totalBestTime[method] = temp.Value;
                        else
                            totalBestTime.Remove(method);
                    }
                Match match = SubsplitRegex.Match(Run[lastidx].Name);
                TimeSpan? nullTS = null;
                result.AddSegment(match.Success ? match.Groups[1].Value : Run[lastidx].Name,
                                  pbSplitTime: Run[lastidx].PersonalBestSplitTime,
                                  bestSegmentTime: totalBestTime != null ? new Time(totalBestTime.ContainsKey(TimingMethod.RealTime) ? totalBestTime[TimingMethod.RealTime] : nullTS,
                                                                                    totalBestTime.ContainsKey(TimingMethod.GameTime) ? totalBestTime[TimingMethod.GameTime] : nullTS)
                                                                         : new Time(),
                                  icon: Run.GameIcon,
                                  splitTime: Run[lastidx].SplitTime,
                                  segmentHistory: curList);
            }

            foreach (var generator in result.ComparisonGenerators)
            {
                generator.Run = result;
            }

            return result;
        }

        private static LiveSplitState prevState = null; // trimmed
        private static int prevCurrentSplitIndex = -1; //not trimmed
        private static int prevSplitCount = -1; //not trimmed
        public static LiveSplitState fromSubsplitToSplit(LiveSplitState state)
        {
            return prevState = updateNoSubsplit(state, prevState);
        }

        private static LiveSplitState updateNoSubsplit(LiveSplitState state, LiveSplitState prevResult) // Layout, LayoutSettings and Settings may be incorrect, do not rely on this for these.
        {
            if (state == null)
                return null;

            LiveSplitState result = prevResult != null ? prevResult : (LiveSplitState)state.Clone();
            // not copying Run and CurrentSplitIndex
            result.Form = state.Form;
            result.Layout = state.Layout;
            result.Settings = state.Settings;
            result.LayoutSettings = state.LayoutSettings;
            result.AdjustedStartTime = state.AdjustedStartTime;
            result.StartTimeWithOffset = state.StartTimeWithOffset;
            result.StartTime = state.StartTime;
            result.TimePausedAt = state.TimePausedAt;
            result.LoadingTimes = state.LoadingTimes;
            result.IsGameTimePaused = state.IsGameTimePaused;
            result.GameTimePauseTime = state.GameTimePauseTime;
            result.CurrentPhase = state.CurrentPhase;
            result.CurrentComparison = state.CurrentComparison;
            result.CurrentHotkeyProfile = state.CurrentHotkeyProfile;
            result.CurrentTimingMethod = state.CurrentTimingMethod;
            result.AttemptStarted = state.AttemptStarted;
            result.AttemptEnded = state.AttemptEnded;

            bool sameSplit = true;
            if (prevResult == null || prevCurrentSplitIndex != state.CurrentSplitIndex || prevSplitCount != state.Run.Count)
            {
                sameSplit = false;
                List<int> noSubsplitIndexes = new List<int>();
                for (var idx = 0; idx < state.Run.Count; idx++)
                {
                    bool isSubsplit = state.Run[idx].Name.StartsWith("-") && idx != state.Run.Count - 1;
                    if (!isSubsplit)
                        noSubsplitIndexes.Add(idx);
                }
                result.CurrentSplitIndex = -1;
                for (var idx2 = 0; idx2 < noSubsplitIndexes.Count; idx2++)
                {
                    var startidx = idx2 > 0 ? noSubsplitIndexes[idx2 - 1] + 1 : 0;
                    var lastidx = noSubsplitIndexes[idx2];
                    if (state.CurrentSplitIndex >= startidx && state.CurrentSplitIndex <= lastidx)
                    {
                        result.CurrentSplitIndex = idx2;
                    }
                }
                if (state.CurrentSplitIndex >= state.Run.Count)
                {
                    result.CurrentSplitIndex = noSubsplitIndexes.Count;
                }
                else if (result.CurrentSplitIndex == -1)
                {
                    result.CurrentSplitIndex = state.CurrentSplitIndex;
                }
            }

            if (state.Run.HasChanged || !sameSplit)
                result.Run = updateNoSubsplit(state.Run, prevResult != null ? prevResult.Run : null, out changed, sameSplit);//prevCurrentSplitIndex == state.CurrentSplitIndex);

            if (changed || !sameSplit)
            {
                foreach (var generator in result.Run.ComparisonGenerators)
                {
                    generator.Generate(result.Settings);
                }
            }
            prevCurrentSplitIndex = state.CurrentSplitIndex;
            prevSplitCount = state.Run.Count;
            return result;
        }
    }
}
