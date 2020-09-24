using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MBServerMonitor
{
    public enum SessionState
    {
        Idle,
        Processing,
        Stopping
    }
    public class Session
    {
        private ListView lsvServerInfomation;
        private SessionState state;
        public Stack<Thread> subThreads;
        private delegate void ListViewAddItemDelegate(ListViewItem item);
        private ListViewAddItemDelegate listViewAddItemDelegate;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public Session(ref DoubleBufferListView lsvServerInfomation)
        {
            state = SessionState.Idle;
            this.lsvServerInfomation = lsvServerInfomation;
            subThreads = new Stack<Thread>();
            listViewAddItemDelegate = new ListViewAddItemDelegate(ListViewAddItemDelegateMethod);
            timer = new System.Windows.Forms.Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 10;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (subThreads.Count > 0)
            {
                subThreads.Pop().Start();
            }
        }

        public void StartFetch(int type)
        {
            state = SessionState.Processing;
            subThreads.Clear();
            lsvServerInfomation.Items.Clear();
            Thread thread = new Thread(() =>
            {
                refreshGameServersList(type);
            });
            thread.Start();
        }

        public void EndSession()
        {
            state = SessionState.Stopping;
            for (int i = 0; i < subThreads.Count; i++)
            {
                subThreads.ElementAt(i).Abort();
            }
            subThreads.Clear();
            lsvServerInfomation.Items.Clear();
        }

        private void refreshGameServersList(int gameTypeIndex)
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
                foreach (var token in tokens)
                {
                    Thread newThread = new Thread(() =>
                    {
                        fetchGameServerInfo(token);
                    });
                    subThreads.Push(newThread);
                }
            }
            catch { }
        }

        private void fetchGameServerInfo(string serverIPWithPort)
        {
            if (state == SessionState.Stopping)
            {
                return;
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{0}",
                   serverIPWithPort));
                request.UserAgent = "UserAgent";
                request.Timeout = int.MaxValue;
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                string responseTxt = null;
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseTxt = reader.ReadToEnd();
                }

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
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = eleName.InnerText;
                    lvi.SubItems.Add(eleModule.InnerText);
                    lvi.SubItems.Add(eleMap.InnerText);
                    lvi.SubItems.Add(eleMapType.InnerText);
                    lvi.SubItems.Add(string.Format("{0}/{1}", eleActivePlayer.InnerText, eleMaxNumberPlayer.InnerText));
                    lvi.SubItems.Add(eleDedicated.InnerText);
                    lvi.SubItems.Add(eleHasPassword.InnerText);
                    ListViewAddItemDelegateMethod(lvi);
                }
            }
            catch
            {

            }
        }

        private void ListViewAddItemDelegateMethod(ListViewItem item)
        {
            if (state == SessionState.Stopping)
            {
                return;
            }

            if (lsvServerInfomation.InvokeRequired)
            {
                lsvServerInfomation.Invoke(listViewAddItemDelegate, item);
            }
            else
            {
                lsvServerInfomation.Items.Add(item);
            }
        }
    }
}
