using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVELogClient
{
    delegate void ShowError(string err);
    delegate void ShowSettings();

    public partial class EVEIntel : Form
    {
        public EVEIntel()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Settings().ShowDialog();
        }

        public void updateStatus(string s)
        {
            status.Text = s;
        }

        public void showError(string err)
        {
            MessageBox.Show(this, err);
        }

        public void showSettings()
        {
            new Settings().ShowDialog();
        }
    }
}
