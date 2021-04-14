using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MBServerMonitor
{
    public class MBServerInfoGetor
    {
        public MBServerInfoGetor()
        {

        }

        public List<string> GetGameServersList(int gameTypeIndex)
        {
            try
            {
                string gameType = null;
                if (gameTypeIndex == 0)
                {
                    gameType = "wb";
                }
                else if (gameTypeIndex == 1)
                {
                    gameType = "wfas";
                }
                else
                {
                    gameType = "unknown";
                }

                string reqUrl = string.Format("http://warbandmain.taleworlds.com/handlerservers.ashx?type=list&gametype={0}", gameType);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqUrl);
                request.UserAgent = "UserAgent";
                request.Timeout = 120000;
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                string responseTxt = null;
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseTxt = reader.ReadToEnd();
                }

                string[] tokens = responseTxt.Split('|');
                return tokens.ToList();
            }
            catch
            {
                return null;
            }
        }

        public ServerInfo GetGameServerInfo(string serverIPWithPort)
        {
            try
            {
                var s = new Socket(SocketType.Stream, ProtocolType.Tcp);
                var tokens = serverIPWithPort.Split(":");
                s.Connect(tokens[0], int.Parse(tokens[1]));

                var request = "GET " + serverIPWithPort;
                var outBytes = Encoding.UTF8.GetBytes(request);
                var outBuffer = new ArraySegment<byte>(outBytes);

                s.Send(outBuffer, SocketFlags.None);

                var inBytes = new byte[1024*2];
                var inBuffer = new ArraySegment<byte>(inBytes);
                s.Receive(inBuffer, SocketFlags.None);

                var responseTxt = Encoding.UTF8.GetString(inBuffer.Array);

                responseTxt = responseTxt.Substring(responseTxt.IndexOf("<ServerStats>")).Trim();

                XmlDocument document = new XmlDocument();
                document.LoadXml(responseTxt);
                var eleName = document.GetElementsByTagName("Name")[0];
                var eleModule = document.GetElementsByTagName("ModuleName")[0];
                var eleMap = document.GetElementsByTagName("MapName")[0];
                var eleMapType = document.GetElementsByTagName("MapTypeName")[0];
                var eleActivePlayer = document.GetElementsByTagName("NumberOfActivePlayers")[0];
                var eleMaxNumberPlayer = document.GetElementsByTagName("MaxNumberOfPlayers")[0];
                var eleDedicated = document.GetElementsByTagName("IsDedicated")[0];
                var eleHasPassword = document.GetElementsByTagName("HasPassword")[0];

                if (eleName != null)
                {
                    ServerInfo si = new ServerInfo(serverIPWithPort);
                    si.Name = eleName.InnerText;
                    si.Module = eleModule.InnerText;
                    si.Map = eleMap.InnerText;
                    si.MapType = eleMapType.InnerText;
                    si.ActivePlayers = eleActivePlayer.InnerText;
                    si.MaxPlayers = eleMaxNumberPlayer.InnerText;
                    if (eleDedicated != null)
                    {
                        si.Dedicated = eleDedicated.InnerText;
                    }
                    if (eleHasPassword != null)
                    {
                        si.HasPassword = eleHasPassword.InnerText;
                    }
                    return si;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
