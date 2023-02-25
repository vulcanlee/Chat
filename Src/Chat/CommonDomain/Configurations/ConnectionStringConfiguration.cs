using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDomain.Configurations
{
    public class ConnectionStringConfiguration
    {
        public string ChatDefaultConnection { get; set; }=string.Empty;
        public string ChatSQLiteDefaultConnection { get; set; }= string.Empty;
    }
}
