using System.Linq;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Database
{
    public class PrecalculationResult
    {
        public PrecalculationResult(Workspace workspace, WorkspaceAnalysisResult result)
        {
            Result = result;
            VotesCount = workspace.Votes?.Select(x => x.ApplicationUserId).Distinct().Count() ?? 0;
            if (result.StakeholdersConsensus?.Count > 0)
            {
                StakeholderConsensusAvg = result.StakeholdersConsensus.Average(x => x.Value);
            }
        }

        public WorkspaceAnalysisResult Result { get; }
        public int VotesCount { get; }
        public double? StakeholderConsensusAvg { get; }
    }
}
