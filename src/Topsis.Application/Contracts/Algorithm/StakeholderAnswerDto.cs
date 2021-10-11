using Topsis.Domain;

namespace Topsis.Application.Contracts.Algorithm
{
    public class StakeholderAnswerDto
    {
        public int WorkspaceId { get; set; }
        public string StakeholderId { get; set; }
        public int AlternativeId { get; set; }
        public int CriterionId { get; set; }
        public int CriterionWeight { get; set; }
        
        public double AnswerValue { get; set; }
    }
}
