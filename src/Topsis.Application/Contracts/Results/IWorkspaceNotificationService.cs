using System.Threading.Tasks;
using Topsis.Domain;
using Topsis.Domain.Common;

namespace Topsis.Application.Contracts.Results
{
    public struct WorkspaceStatusChangedMessage
    {
        public WorkspaceStatusChangedMessage(int workspaceId, WorkspaceStatus status)
        {
            WorkspaceId = workspaceId.Hash();
            Status = status;
        }

        public string WorkspaceId { get; }
        public WorkspaceStatus Status { get; }
    }

    public struct WorkspaceNotificationMessage
    {
        public WorkspaceNotificationMessage(int workspaceId, string title, string message)
        {
            WorkspaceId = workspaceId.Hash();
            Title = title;
            Message = message;
        }

        public string WorkspaceId { get; }
        public string Title { get; }
        public string Message { get; }
    }

    public interface IWorkspaceNotificationService
    {
        Task OnWorkspaceStatusChangedAsync(WorkspaceStatusChangedMessage message);
        Task OnWorkspaceMessageSendAsync(WorkspaceNotificationMessage message);
    }
}
