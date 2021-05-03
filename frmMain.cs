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
    public partial class frmMain : Form
    {
        private int sortDirection = 0;
        private List<ServerInfo> serverInfos;

        public frmMain()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            cmbGameTypeList.SelectedIndex = 0;
        }

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex == this.dataGridView1.RowCount - 1) return;

            var si = serverInfos[e.RowIndex];

            switch (dataGridView1.Columns[e.ColumnIndex].Name)
            {
                case "col_name":
                    e.Value = si.Name;
                    break;
                case "col_module":
                    e.Value = si.Module;
                    break;
                case "col_map":
                    e.Value = si.Map;
                    break;
                case "col_mapType":
                    e.Value = si.MapType;
                    break;
                case "col_players":
                    e.Value = si.ActivePlayers + "/" + si.MaxPlayers;
                    break;
                case "col_dedicated":
                    e.Value = si.Dedicated;
                    break;
                case "col_haspassword":
                    e.Value = si.HasPassword;
                    break;
            }
        }

        private void dataGridView1_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            btnRefresh.Enabled = false;
            frmDownloadingForm downloadingForm = new frmDownloadingForm(cmbGameTypeList.SelectedIndex);
            if (downloadingForm.ShowDialog() == DialogResult.OK)
            {
                serverInfos = downloadingForm.ServerInfos;
                dataGridView1.DataSource = serverInfos;
                dataGridView1.Refresh();
            }
            btnRefresh.Enabled = true;
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.Sort(dataGridView1.Columns[e.ColumnIndex], (ListSortDirection)sortDirection);

            if (sortDirection == 0)
            {
                sortDirection = 1;
            }
            else if (sortDirection == 1)
            {
                sortDirection = 0;
            }
        }
    }
}
