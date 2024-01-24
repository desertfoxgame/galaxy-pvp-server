using Microsoft.VisualBasic.FileIO;
using System.Reflection;

namespace GalaxyPvP.Helper
{
    public class GalaxyExtensions
    {
        public static List<T> GetListDataCsv<T>() where T : new ()
        {
            // Get the current directory of the application
            string currentDirectory = Directory.GetCurrentDirectory() + "\\DataID - Assetsitemdata.csv";

            var data = new List<T>();

            using (var parser = new TextFieldParser(currentDirectory))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    var item = new T();

                    PropertyInfo[] properties = typeof(T).GetProperties();
                    for (int i = 0; i < fields.Length && i < properties.Length; i++)
                    {
                        properties[i].SetValue(item, Convert.ChangeType(fields[i], properties[i].PropertyType));
                    }

                    data.Add(item);
                }
            }

            return data;
        }

        //public static void SaveDictionaryToCache(Dictionary<string, MatchMakingTicket> data)
        //{
        //    // Set the dictionary in the cache with a specific key and expiration time
        //    _memoryCache.Set("PlayerPools", data);
        //}

        //public static Dictionary<string, MatchMakingTicket> GetCachedDictionary()
        //{
        //    // Try to get the dictionary from the cache
        //    if (_memoryCache.TryGetValue("PlayerPools", out Dictionary<string, MatchMakingTicket> cachedData))
        //    {
        //        return cachedData;
        //    }

        //    // If not in the cache, return null or fetch it from the data source
        //    return null;
        //}
    }
}
