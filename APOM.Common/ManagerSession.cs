using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace APOM.Common
{
    public static class ManagerSession
    {
        public static string GetUser()
        {
            return System.Environment.UserName.ToString();
        }

        public static string GetMachine()
        {
            return Environment.MachineName.ToString();
        }

        public static string GetIp()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        }
    }
}
