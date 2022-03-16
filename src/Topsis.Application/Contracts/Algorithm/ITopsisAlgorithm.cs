using System.Collections.Generic;
using System.Threading.Tasks;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Algorithm
{

    public interface ITopsisAlgorithm
    {
        Task<WorkspaceAnalysisResult> AnalyzeAsync(Workspace workspace, 
            IDictionary<int, string> jobCategories, 
            IList<StakeholderAnswerDto> answers,
            IList<StakeholderDemographicsDto> stakeholdersDemographics);
    }
}
