namespace GalaxyPvP.Api.Services
{
    public class ConnectionIdService
    {
        private readonly Dictionary<string, string> ConnectionIdMap = new Dictionary<string, string>();
        private readonly object lockObject = new object();

        public void AddConnectionId(string userId, string connectionId)
        {
            lock (lockObject)
            {
                ConnectionIdMap[userId] = connectionId;
            }
        }

        public void RemoveConnectionId(string userId)
        {
            lock (lockObject)
            {
                ConnectionIdMap.Remove(userId);
            }
        }

        public string GetConnectionId(string userId)
        {
            lock (lockObject)
            {
                return ConnectionIdMap.TryGetValue(userId, out var connectionId) ? connectionId : null;
            }
        }

        public Dictionary<string, string> GetAllConnection()
        {
            lock (lockObject)
            {
                return ConnectionIdMap == null ? new Dictionary<string, string>() : ConnectionIdMap;
            }
        }
    }
}
