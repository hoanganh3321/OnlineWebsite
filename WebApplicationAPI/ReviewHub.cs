using Microsoft.AspNetCore.SignalR;

namespace WebApplicationAPI
{
    public class ReviewHub : Hub
    {
        public async Task JoinProductGroup(string foodId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"product_{foodId}");
        }

        public async Task LeaveProductGroup(string foodId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"product_{foodId}");
        }
    }
}
