using Topsis.Domain;

namespace Topsis.Application.Contracts.Database
{
    public class StakeholderWorkspaceDto
    {
        public int WorkspaceId { get; set; }
        public string WorkspaceTitle { get; set; }
        public string WorkspaceDescription { get; set; }
        public WorkspaceStatus CurrentStatus { get; set; }
        public int? VoteId { get; set; }
        public string StakeholderId { get; set; }
    }
}
