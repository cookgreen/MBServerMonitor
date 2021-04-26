using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MBServerMonitor
{
    public partial class frmDownloadingForm : Form
    {
        private DownloadServerInfoTask downloadTask;
        
        private delegate void LabelSetTextDelegate(Label label, string str);
        private LabelSetTextDelegate labelSetTextDelegate;

        private delegate void ProgressBarSetValueDelegate(ProgressBar progressBar, int value);
        private ProgressBarSetValueDelegate progressBarSetValueDelegate;
        private ProgressBarSetValueDelegate progressBarSetMaxiumDelegate;

        public List<ServerInfo> ServerInfos
        {
            get { return downloadTask.ServerInfos; }
        }

        public frmDownloadingForm(int gameType)
        {
            InitializeComponent();

            labelSetTextDelegate = new LabelSetTextDelegate(SetLabelText);
            progressBarSetValueDelegate = new ProgressBarSetValueDelegate(SetProgressBarValue);
            progressBarSetMaxiumDelegate = new ProgressBarSetValueDelegate(SetProgressBarMaxium);

            downloadTask = new DownloadServerInfoTask();
            downloadTask.DownloadFinished += DownloadTask_DownloadFinished;
            downloadTask.DownloadProgressReport += DownloadTask_DownloadProgressReport;
            downloadTask.Start(gameType);

            lbStageName.Text = null;
        }

        private void DownloadTask_DownloadProgressReport(string stage, int current, int maxium)
        {
            SetLabelText(lbStageName, stage);
            SetLabelText(label1, string.Format("{0}/{1}", current, maxium));
            SetProgressBarMaxium(progressBar1, maxium);
            SetProgressBarValue(progressBar1, current);
        }

        private void SetProgressBarMaxium(ProgressBar progressBar, int maxium)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(progressBarSetMaxiumDelegate, progressBar, maxium);
            }
            else
            {
                progressBar.Maximum = maxium;
            }
        }

        private void SetProgressBarValue(ProgressBar progressBar, int value)
        {
            if(progressBar.InvokeRequired)
            {
                progressBar.Invoke(progressBarSetValueDelegate, progressBar, value);
            }
            else
            {
                progressBar.Value = value;
            }
        }

        private void SetLabelText(Label label, string text)
        {
            if(label.InvokeRequired)
            {
                label.Invoke(labelSetTextDelegate, label, text);
            }
            else
            {
                label.Text = text;
            }
        }

        private void DownloadTask_DownloadFinished()
        {
            DialogResult = DialogResult.OK;

            Close();
        }
    }
}
