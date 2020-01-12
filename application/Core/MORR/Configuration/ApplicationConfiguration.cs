using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MORR.Shared.Utility;

namespace MORR.Core.Configuration
{
    public class ApplicationConfiguration
    {
         public FilePath SaveLocation { get; set; }
         public string SaveName { get; set; }

         public ApplicationConfiguration(FilePath saveLocation, string saveName)
         {
             SaveLocation = saveLocation;
             SaveName = saveName;
         }
    }
}
