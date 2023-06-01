using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Topsis.Domain.Common;

namespace Topsis.Web.Hubs
{
    public class VotingHub : Hub
    {
        // https://learn.microsoft.com/en-us/aspnet/core/signalr/javascript-client?view=aspnetcore-7.0&tabs=visual-studio

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            // await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public static string GetWorkspaceGroupName(int workspaceId)
        {
            return $"workspace-{workspaceId.Hash()}".ToLower();
        }
    }
}
