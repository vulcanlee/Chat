using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDomain.Configurations
{
    public class ChatSystemAssistantConfiguration
    {
        public bool UseSQLite { get; set; }=false;
        public string UploadImagesPath { get; set; }= string.Empty;
        public string UploadCommonFilePath { get; set; }= string.Empty;
    }
}
