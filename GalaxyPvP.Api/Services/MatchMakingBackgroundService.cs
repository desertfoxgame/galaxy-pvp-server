using Azure.Core;
using GalaxyPvP.Api.Hubs;
using GalaxyPvP.Data.Model;
using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GalaxyPvP.Api.Services
{
    public class MatchMakingBackgroundService : BackgroundService
    {
        private readonly ILogger<MatchMakingBackgroundService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<MatchMakingHub> _hubContext;
        private readonly ConnectionIdService _connectionIdService;

        public MatchMakingBackgroundService(ILogger<MatchMakingBackgroundService> logger, IMemoryCache memoryCache, IHubContext<MatchMakingHub> hubContext, ConnectionIdService connectionIdService)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _hubContext = hubContext;
            _connectionIdService = connectionIdService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Dictionary<string, MatchMakingTicket> playerPool = GetPlayerPool();
                if (playerPool == null)
                {
                    playerPool = new Dictionary<string, MatchMakingTicket>();
                    SavePlayerPool(playerPool);
                }

                //_logger.LogInformation($"Counter: {playerPool.Count}");

                foreach (MatchMakingTicket ticket in playerPool.Values)
                {
                    string connectionId = _connectionIdService.GetConnectionId(ticket.PlayerId);

                    if (connectionId != null)
                    {
                        // Send a message to the specified user using their ConnectionId
                        await _hubContext.Clients.Client(connectionId).SendAsync("ServerMatchMakingMessage", null);
                    }
                    else
                    {
                        // Handle the case where the user is not connected
                        // You may want to log this or take other appropriate action
                        Console.WriteLine($"User with ID {ticket.PlayerId} is not currently connected.");
                    }
                }

                //_logger.LogInformation($"{_connectionIdService.GetAllConnection().Count}");

                if (playerPool.Count >= 4)
                {
                    // Matchmaking logic
                    List<List<MatchMakingTicket>> teams = FormTeams(playerPool);

                    // Notify players about the match
                    NotifyPlayers(teams);

                    // Remove matched players from the pool
                    RemovePlayersFromPool(playerPool, teams);

                    //_logger.LogInformation($"Match Found: {teams.Count} teams formed.");
                }

                // Adjust the delay to run every 5 seconds
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private List<List<MatchMakingTicket>> FormTeams(Dictionary<string, MatchMakingTicket> playerPool)
        {
            List<MatchMakingTicket> sortedPool = SortPlayerPool(playerPool);

            int teamSize = 6;
            int totalPlayers = sortedPool.Count;
            int totalTeams = totalPlayers / teamSize;
            if (totalTeams == 0 && playerPool.Count >= 4 && DateTime.Now - playerPool.Values.First().SubmitedTime > TimeSpan.FromSeconds(30))
            {
                totalTeams = 1;
            }

            List<List<MatchMakingTicket>> teams = new List<List<MatchMakingTicket>>();

            for (int i = 0; i < totalTeams; i++)
            {
                List<MatchMakingTicket> team = sortedPool.Skip(i * teamSize).Take(teamSize).ToList();
                team.FirstOrDefault().IsHost = true;
                // Fill in bots if the team size is below the minimum and over 30 seconds
                if (team.Count >= 4 && team.Count < 6)
                {
                    int botsNeeded = 6 - team.Count;
                    for (int j = 0; j < botsNeeded; j++)
                    {
                        MatchMakingTicket botTicket = new MatchMakingTicket
                        {
                            PlayerId = $"Bot{j + 1}",

                        };
                        team.Add(botTicket);
                    }
                }
                teams.Add(team);
            }

            return teams;
        }

        private List<MatchMakingTicket> SortPlayerPool(Dictionary<string, MatchMakingTicket> playerPool)
        {
            return playerPool.Values.OrderBy(player => player.Trophy).ToList();
        }

        private async void NotifyPlayers(List<List<MatchMakingTicket>> teams)
        {
            string json = ConvertTeamsToJson(teams);
            foreach (var team in teams)
            {
                foreach (var player in team)
                {
                    string connectionId = _connectionIdService.GetConnectionId(player.PlayerId);

                    if (connectionId != null)
                    {
                        // Send a message to the specified user using their ConnectionId
                        await _hubContext.Clients.Client(connectionId).SendAsync("ServerMatchMakingMessage", json);
                    }
                    else
                    {
                        // Handle the case where the user is not connected
                        // You may want to log this or take other appropriate action
                        Console.WriteLine($"User with ID {player.PlayerId} is not currently connected.");
                    }
                }
            }
        }

        private void RemovePlayersFromPool(Dictionary<string, MatchMakingTicket> playerPool, List<List<MatchMakingTicket>> teams)
        {
            foreach (var team in teams)
            {
                foreach (var player in team)
                {
                    playerPool.Remove(player.PlayerId);
                }
            }

            SavePlayerPool(playerPool);
        }

        Dictionary<string, MatchMakingTicket> GetPlayerPool()
        {
            // Try to get the dictionary from the cache
            if (_memoryCache.TryGetValue("PlayerPools", out Dictionary<string, MatchMakingTicket> cachedData))
            {
                return cachedData;
            }

            // If not in the cache, return null or fetch it from the data source
            return null;
        }

        void SavePlayerPool(Dictionary<string, MatchMakingTicket> data)
        {
            // Set the dictionary in the cache with a specific key and expiration time
            _memoryCache.Set("PlayerPools", data);
        }

        private string ConvertTeamsToJson(List<List<MatchMakingTicket>> teams)
        {
            List<List<Dictionary<string, object>>> playerIdsAndTrophies = teams
                    .Select(team => team.Select(ticket => new Dictionary<string, object> { { "PlayerId", ticket.PlayerId }, { "Trophy", ticket.Trophy }, {"IsHost", ticket.IsHost} }).ToList())
                    .ToList();
            // Serialize the list of teams to JSON
            string json = JsonSerializer.Serialize(playerIdsAndTrophies);

            return json;
        }
    }
}
