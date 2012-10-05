using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVELogClient
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            this.channel.KeyDown += addChannel;
            this.channelList.KeyDown += removeChannel;
            this.ControlBox = false;
            populateFields();
        }


        private void populateFields()
        {
            this.username.Text = IntelProperties.getProperty("USER_ID");
            this.key.Text = IntelProperties.getProperty("USER_KEY");
            this.logDir.Text = IntelProperties.getProperty("LOG_DIR");

            this.channelList.Items.Clear();
            foreach (string channel in IntelProperties.getProperty("CHANNELS").Split(','))
            {
                this.channelList.Items.Add(channel.Trim());
            }
        }

        private void addChannel(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string txt = this.channel.Text;
                this.channelList.Items.Add(txt);
                this.channel.Text = "";
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void removeChannel(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ListBox lb = (ListBox)sender;
                if (lb.SelectedIndex >= 0)
                {
                    lb.Items.RemoveAt(lb.SelectedIndex);
                }
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            IntelProperties.setProperty("USER_ID", this.username.Text);
            IntelProperties.setProperty("USER_KEY", this.key.Text);

            if (!Directory.Exists(this.logDir.Text))
            {
                MessageBox.Show(this, "Log directory not found");
                return;
            }
            IntelProperties.setProperty("LOG_DIR", this.logDir.Text);

            string chnls = "";
            foreach (string channel in this.channelList.Items)
            {
                chnls += "," + channel;
            }
            IntelProperties.setProperty("CHANNELS", chnls.Substring(1));

            if (IntelProperties.validateProperties())
            {
                IntelProperties.save();
                Program.stopMonitor();
                Program.runMonitor();
                this.Hide();
            }
            else
            {
                MessageBox.Show(this, "Please fill in all required fields");
            }
        }

    }
}
