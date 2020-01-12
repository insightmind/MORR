using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using MORR.Shared.Utility;

namespace MORR.Core.Configuration
{
    public class ApplicationConfiguration
    {
        private const string saveLocationKey = "SaveLocation";
        private const string saveNameKey = "SaveName";

        public FilePath SaveLocation { get; set; } 
        public string SaveName { get; set; }
        
        public ApplicationConfiguration(FilePath saveLocation, string saveName) 
        {
            SaveLocation = saveLocation;
            SaveName = saveName;
        }

        public ApplicationConfiguration(JsonElement rootElement)
        {
            try
            {
                SaveLocation = new FilePath(rootElement.GetProperty(saveLocationKey).GetString());
                SaveName = rootElement.GetProperty(saveNameKey).GetString();
            }
            catch (Exception exception) 
            {
                throw new InvalidConfigurationException("Invalid root application configuration!", exception);
            }
             
        }
    }
}
