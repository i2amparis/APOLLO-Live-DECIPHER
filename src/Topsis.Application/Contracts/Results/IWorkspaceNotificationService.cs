using System.Threading.Tasks;
using Topsis.Domain;

namespace Topsis.Application.Contracts.Results
{
    public interface IWorkspaceNotificationService
    {
        Task OnWorkspaceStatusChangedAsync(Workspace workspace);
        Task OnWorkspaceMessageSendAsync(Workspace workspace, string title, string message);
    }
}
