namespace GalaxyPvP.Api.Services
{
    public class MatchSubmitDictionary
    {
        private readonly Dictionary<string, string> Dictionary = new Dictionary<string, string>();

        public Dictionary<string, string> GetDictionary()
        {
            return Dictionary;
        }
    }
}
