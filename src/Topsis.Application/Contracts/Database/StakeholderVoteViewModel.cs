using System.Collections.Generic;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Database
{
    public class StakeholderVoteViewModel
    { 
        public IList<KeyValuePair<int, string>> AlternativesOrdered { get; set; }
        public IList<Criterion> CriteriaOrdered { get; set; }
        public List<NameValueOption> AlternativeRange { get; set; }
        public string WorkspaceTitle { get; set; }
        public string WorkspaceId { get; set; }
        public int[] CriteriaImportanceRange { get; set; }
        public StakeholderAnswer[] StakeholderAnswers { get; private set; }

        public void AddStakeholderAnswers(StakeholderAnswer[] answers)
        {
            StakeholderAnswers = answers;
        }
    }
}
