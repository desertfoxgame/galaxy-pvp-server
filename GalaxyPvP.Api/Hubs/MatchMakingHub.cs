using GalaxyPvP.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace GalaxyPvP.Api.Hubs
{
    public class MatchMakingHub : Hub
    {
        private readonly IHubContext<MatchMakingHub> _hubContext;
        private static readonly Dictionary<string, string> PlayerConnections = new Dictionary<string, string>();
        private readonly ConnectionIdService _connectionIdService;

        public MatchMakingHub(IHubContext<MatchMakingHub> hubContext, ConnectionIdService connectionIdService)
        {
            _hubContext = hubContext;
            _connectionIdService = connectionIdService;
        }


        public override Task OnConnectedAsync()
        {
            // Get the player ID from the query string or elsewhere
            string playerId = Context.GetHttpContext().Request.Query["userId"];

            // Store the player's connection ID
            //PlayerConnections[playerId] = Context.ConnectionId;
            _connectionIdService.AddConnectionId(playerId, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string playerId = Context.GetHttpContext().Request.Query["userId"];

            // Remove the ConnectionId from your shared service when the client disconnects
            _connectionIdService.RemoveConnectionId(Context.UserIdentifier);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task ServerMatchMaking(string message)
        {
            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ServerMatchMakingMessage", message);
        }

        public async Task ServerInitiatedMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveServerMessage", message);
        }
    }
}
