using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MBServerMonitor
{
    public class DownloadServerInfoTask
    {
        private BackgroundWorker worker;
        private MBServerInfoFetcher serverInfoFetcher;
        private ServerInfoDAL serverInfoDAL;
        private List<BackgroundWorker> serverInfoFetchThreadPool;
        private List<BackgroundWorker> serverInfoFetchThreadSuccessPool;

        public event Action DownloadFinished;
        public event Action<string, int,int> DownloadProgressReport;

        public List<ServerInfo> ServerInfos;

        public DownloadServerInfoTask()
        {
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            serverInfoFetcher = new MBServerInfoFetcher();
            serverInfoDAL = new ServerInfoDAL();
            serverInfoFetchThreadPool = new List<BackgroundWorker>();
            serverInfoFetchThreadSuccessPool = new List<BackgroundWorker>();
            ServerInfos = new List<ServerInfo>();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DownloadFinished?.Invoke();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            serverInfoDAL.CleanDB();

            DateTime lastActiveProgressReportTime = DateTime.MinValue;
            TimeSpan lastTimeSpan;
            TimeSpan span;

            DownloadProgressReport?.Invoke("Fetching Server Metadata...", 1, 1);
            var servers = serverInfoFetcher.GetGameServersList(int.Parse(e.Argument.ToString()));

            foreach (var server in servers)
            {
                BackgroundWorker thread = new BackgroundWorker();
                thread.DoWork += (o, e) => {
                    var si = serverInfoFetcher.GetGameServerInfo(server);
                    if (si != null)
                    {
                        ServerInfos.Add(si);
                        serverInfoDAL.AddServerInfo(si);
                    }
                };
                thread.RunWorkerCompleted += (o, e) =>
                {
                    serverInfoFetchThreadSuccessPool.Add(thread);
                    DownloadProgressReport?.Invoke("Downloading Server Info...", serverInfoFetchThreadSuccessPool.Count, serverInfoFetchThreadPool.Count);
                    lastActiveProgressReportTime = DateTime.Now;
                };
                thread.RunWorkerAsync();
                serverInfoFetchThreadPool.Add(thread);
            }

            int tick = 0;

            while ((serverInfoFetchThreadPool.Count != serverInfoFetchThreadSuccessPool.Count) && tick!=3)
            {
                if (lastActiveProgressReportTime != DateTime.MinValue)
                {
                    lastTimeSpan = DateTime.Now - lastActiveProgressReportTime;

                    if (lastTimeSpan.TotalSeconds > 1200)
                    {
                        tick++;
                    }

                    span = lastTimeSpan;
                }
            }
        }

        public void Start(int gameType)
        {
            worker.RunWorkerAsync(gameType);
        }
    }
}
