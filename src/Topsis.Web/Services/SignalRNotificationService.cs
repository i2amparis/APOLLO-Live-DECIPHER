using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Results;
using Topsis.Domain;
using Topsis.Domain.Common;
using Topsis.Web.Hubs;

namespace Topsis.Web.Services
{
    public static class SignalRNotificationServiceExtensions
    {
        public static IServiceCollection AddSignalRNotificationService(this IServiceCollection services)
        {
            services.AddTransient<IWorkspaceNotificationService, SignalRNotificationService>();
            return services;
        }
    }

    public class SignalRNotificationService : IWorkspaceNotificationService
    {
        private const string Method_WorkspaceStatusChanged = "workspace_status_changed";
        private readonly IHubContext<VotingHub> _voting;

        public SignalRNotificationService(IHubContext<VotingHub> voting)
        {
            _voting = voting;
        }

        public async Task OnWorkspaceStatusChangedAsync(Workspace workspace)
        {
            var groupName = VotingHub.GetWorkspaceGroupName(workspace.Id);
            await _voting.Clients.Group(groupName).SendAsync(Method_WorkspaceStatusChanged, 
                workspace.Id.Hash(), 
                (short)workspace.CurrentStatus);
        }
    }
}
