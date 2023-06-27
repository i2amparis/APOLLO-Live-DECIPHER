using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Results;
using Topsis.Domain;
using Topsis.Domain.Common;
using Topsis.Domain.Contracts;
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
        private const string Method_WorkspaceSendMessage = "workspace_message";

        private readonly IHubContext<VotingHub> _voting;
        private readonly IUserContext _userContext;

        public SignalRNotificationService(IHubContext<VotingHub> voting, IUserContext userContext)
        {
            _voting = voting;
            _userContext = userContext;
        }

        public async Task OnWorkspaceStatusChangedAsync(WorkspaceStatusChangedMessage message, bool authorizeUser)
        {
            if (authorizeUser)
            {
                AllowModerator(_userContext);
            }

            var groupName = VotingHub.GetWorkspaceGroupName(message.WorkspaceId);
            await _voting.Clients.Group(groupName).SendAsync(Method_WorkspaceStatusChanged, message);
        }

        public async Task OnWorkspaceMessageSendAsync(WorkspaceNotificationMessage message)
        {
            AllowModerator(_userContext);

            var groupName = VotingHub.GetWorkspaceGroupName(message.WorkspaceId);
            await _voting.Clients.Group(groupName).SendAsync(Method_WorkspaceSendMessage, message);
        }

        private void AllowModerator(IUserContext userContext)
        {
            if (userContext.IsInRole(RoleNames.Moderator))
            {
                return;
            }

            throw new UnauthorizedAccessException();
        }

    }
}
