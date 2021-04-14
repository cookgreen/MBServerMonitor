using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBServerMonitor
{
    public class ServerInfo
    {
        private string address;

        public string Address { get { return address; } }
        public string Name { get; set; }
        public string Map { get; set; }
        public string Module { get; set; }
        public string ActivePlayers { get; set; }
        public string MaxPlayers { get; set; }
        public string HasPassword { get; set; }
        public string Dedicated { get; internal set; }
        public string MapType { get; internal set; }

        public ServerInfo(string address)
        {
            this.address = address;
            ActivePlayers = "0";
            MaxPlayers = "0";
            Dedicated = "No";
            HasPassword = "No";
        }
    }
}
