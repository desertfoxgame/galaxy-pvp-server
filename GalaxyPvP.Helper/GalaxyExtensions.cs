using Microsoft.VisualBasic.FileIO;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        public static int GetRewardTrophy(int inputTrophy, bool isWon)
        {
            if (inputTrophy >= 0 && inputTrophy < 100)
            {
                if (isWon) return 8;
                else return 0;
            }
            //Bronze
            else if (inputTrophy > 99 && inputTrophy < 300)
            {
                if (isWon) return 8;
                else return -2;
            }
            //Silver
            else if (inputTrophy > 299 && inputTrophy < 500)
            {
                if (isWon) return 8;
                else return -4;
            }
            //Gold
            else if (inputTrophy > 499 && inputTrophy < 700)
            {
                if (isWon) return 8;
                else    return -6;
            }
            //Platinum
            else if (inputTrophy > 699 && inputTrophy < 900)
            {
                if (isWon) return 7;
                else return -8;
            }
            //Diamond
            else if (inputTrophy > 899 && inputTrophy < 1100)
            {
                if (isWon) return 6;
                else return -10;
            }
            //Master
            else if (inputTrophy > 1099 && inputTrophy < 1300)
            {
                if (isWon) return 5;
                else return -12;
            }
            //Champion
            else if (inputTrophy > 1299 && inputTrophy < 1500)
            {
                if (isWon) return 4;
                else return -14;
            }
            else if (inputTrophy > 1499)
            {
                if (isWon) return 3;
                else return  -14;
            }
            else
            {
                if (isWon) return 2;
                else return -16;
            }
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
