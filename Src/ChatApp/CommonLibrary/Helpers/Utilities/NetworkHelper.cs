using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Helpers.Utilities
{
    public class NetworkHelper
    {
        /// <summary>
        /// Gets if there is an active internet connection
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected()
        {
            var current = Connectivity.Current.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
