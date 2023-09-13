using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class RunPredictionNoSubsplitsFactory : IComponentFactory
    {
        public string ComponentName => "Run Prediction (No Subsplits)";

        public string Description => "Displays what the final run time would be if the run continues at the same pace as a set comparison.";

        public ComponentCategory Category => ComponentCategory.Information;

        public IComponent Create(LiveSplitState state) => new RunPredictionNoSubsplits(state);

        public string UpdateName => ComponentName;

        public string XMLURL => NoSubsplitsHelper.NoSubsplitsHelper.XMLURL;

        public string UpdateURL => NoSubsplitsHelper.NoSubsplitsHelper.UpdateURL;

        public Version Version => Version.Parse(NoSubsplitsHelper.NoSubsplitsHelper.Version);
    }
}
