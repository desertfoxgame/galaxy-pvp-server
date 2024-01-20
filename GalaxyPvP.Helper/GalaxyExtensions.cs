﻿using Microsoft.VisualBasic.FileIO;
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
    }
}