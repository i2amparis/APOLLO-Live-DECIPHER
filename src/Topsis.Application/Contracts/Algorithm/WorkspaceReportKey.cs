using Topsis.Domain.Common;

namespace Topsis.Application.Contracts.Algorithm
{
    //public class WorkspaceResultsDto
    //{
    //    public int Id { get; set; }
    //    public string Title { get; set; }
    //}

    public struct WorkspaceReportKey
    {
        public WorkspaceReportKey(int workspaceId, int workspaceReportId)
        {
            WorkspaceId = workspaceId;
            WorkspaceReportId = workspaceReportId;
        }

        public int WorkspaceId { get; }
        public int WorkspaceReportId { get; }

        public override string ToString()
        {
            return $"ws:{WorkspaceId},{WorkspaceId.Hash()}|r:{WorkspaceReportId},{WorkspaceReportId.Hash()}";
        }
    }
}
