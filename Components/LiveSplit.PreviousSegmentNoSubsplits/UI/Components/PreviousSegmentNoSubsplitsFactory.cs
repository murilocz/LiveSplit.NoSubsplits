using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;

namespace LiveSplit.PreviousSegmentNoSubsplits.UI.Components
{
    public class PreviousSegmentNoSubsplitsFactory : IComponentFactory
    {
        public string ComponentName => "Previous Segment (No Subsplits)";

        public string Description => "Displays how much time was saved or lost on the previous segment in relation to a comparison.";

        public ComponentCategory Category => ComponentCategory.Information;

        public IComponent Create(LiveSplitState state) => new PreviousSegmentNoSubsplits(state);

        public string UpdateName => ComponentName;

        public string XMLURL => NoSubsplitsHelper.NoSubsplitsHelper.XMLURL;

        public string UpdateURL => NoSubsplitsHelper.NoSubsplitsHelper.UpdateURL;

        public Version Version => Version.Parse(NoSubsplitsHelper.NoSubsplitsHelper.Version);
    }
}
