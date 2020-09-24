using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class frmMain : Form
    {
        private Thread mainThread;
        private Stack<Session> sessions;

        public frmMain()
        {
            InitializeComponent();
            sessions = new Stack<Session>();
            cmbGameTypeList.SelectedIndex = 0;


            mainThread = new Thread(() =>
            {
                while (true)
                {
                    lbGameServerNumber.Text = lsvServerInfomation.Items.Count.ToString();
                }
            });
            mainThread.Start();
        }

        private void cmbGameTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sessions.Count > 0)
            {
                sessions.Pop().EndSession();
            }
            Session newSession = new Session(ref lsvServerInfomation);
            newSession.StartFetch(cmbGameTypeList.SelectedIndex);
            sessions.Push(newSession);
            btnRefresh.Text = "Cancel";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (btnRefresh.Text == "Cancel")
            {
                if (sessions.Count > 0)
                {
                    sessions.Pop().EndSession();
                }
                btnRefresh.Text = "Refresh";
            }
            else if (btnRefresh.Text == "Refresh")
            {
                Session newSession = new Session(ref lsvServerInfomation);
                newSession.StartFetch(cmbGameTypeList.SelectedIndex);
                sessions.Push(newSession);
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainThread.Abort();
            for (int i = 0; i < sessions.Count; i++)
            {
                sessions.ElementAt(i).EndSession();
            }
            Application.ExitThread();
        }
    }
}
