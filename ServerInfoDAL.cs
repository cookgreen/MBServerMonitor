using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBServerMonitor
{
    public class ServerInfoDAL
    {
        private SqliteHelper dbHelper;

        public ServerInfoDAL()
        {
            dbHelper = new SqliteHelper("server_db.db");
        }

        public void CleanDB()
        {
            dbHelper.ExecuteSql("delete from ServerInfo");
        }

        public void ResetSequence()
        {
            dbHelper.ExecuteSql("update sqlite_sequence set seq=0 where name='ServerInfo'");
        }

        public void AddServerInfo(ServerInfo serverInfo)
        {
            serverInfo.Name    = Utility.FilterSpecialChars(serverInfo.Name);
            serverInfo.Module  = Utility.FilterSpecialChars(serverInfo.Module);
            serverInfo.Map     = Utility.FilterSpecialChars(serverInfo.Map);
            serverInfo.MapType = Utility.FilterSpecialChars(serverInfo.MapType);

            dbHelper.ExecuteSql("insert into ServerInfo (Address, Name, Module, Map, MapType, ActivePlayer, MaxPlayer, Dedicated, HasPassword) values('" + serverInfo.Address + "','" + serverInfo.Name + "','" + serverInfo.Module + "','" + serverInfo.Map + "','" + serverInfo.MapType + "','" + serverInfo.ActivePlayers + "','" + serverInfo.MaxPlayers + "','" + serverInfo.Dedicated + "','" + serverInfo.HasPassword + "')");
        }

        public List<ServerInfo> GetServerInfoData()
        {
            List<ServerInfo> serverInfos = new List<ServerInfo>();

            var reader = dbHelper.ExecuteSqlReader("select * from ServerInfo");
            while(reader.Read())
            {
                ServerInfo serverInfo = new ServerInfo(reader["Address"].ToString());
                serverInfo.Name = reader["Name"].ToString();
                serverInfo.Module = reader["Module"].ToString();
                serverInfo.Map = reader["Map"].ToString();
                serverInfo.MapType = reader["MapType"].ToString();
                serverInfo.ActivePlayers = reader["ActivePlayer"].ToString();
                serverInfo.MaxPlayers = reader["MaxPlayer"].ToString();
                serverInfo.Dedicated = reader["Dedicated"].ToString();
                serverInfo.HasPassword = reader["HasPassword"].ToString();
                serverInfos.Add(serverInfo);
            }
            reader.Close();

            return serverInfos;
        }
    }
}
