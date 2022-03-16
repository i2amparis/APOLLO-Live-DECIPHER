using System.Collections.Generic;

namespace Topsis.Domain
{
    public class StakeholderVote : Entity
    {
        public StakeholderVote()
        {
            Answers = new List<StakeholderAnswer>();
            CriteriaImportance = new List<StakeholderCriterionImportance>();
        }

        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
        public string ApplicationUserId { get; set; }

        public ICollection<StakeholderAnswer> Answers { get; set; }
        public ICollection<StakeholderCriterionImportance> CriteriaImportance {get;set;} 
        public double? Weight { get; set; }

        public void Accept(IList<StakeholderAnswer> answers, IDictionary<int, int> importance)
        {
            Answers.Clear();

            foreach (var item in answers)
            {
                item.Vote = this;
                Answers.Add(item);
            }

            CriteriaImportance.Clear();
            foreach (var item in importance)
            {
                var criterionImportance = new StakeholderCriterionImportance()
                {
                    Vote = this,
                    CriterionId = item.Key,
                    Weight = item.Value
                };
                CriteriaImportance.Add(criterionImportance);
            }
        }
    }

    public class StakeholderAnswer : Entity
    {
        public int VoteId { get; set; }
        public StakeholderVote Vote { get; set; }

        public int AlternativeId { get; set; }
        public Alternative Alternative { get; set; }

        public int CriterionId { get; set; }
        public Criterion Criterion { get; set; }

        public double Value { get; set; }
    }

    public class StakeholderCriterionImportance : Entity 
    {
        public int VoteId { get; set; }
        public StakeholderVote Vote { get; set; }

        public int CriterionId { get; set; }
        public Criterion Criterion { get; set; }
        public int Weight { get; set; }
    }
}
