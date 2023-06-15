using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Chat_Client
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            // get version number
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            label1.Text = $"Chat Client v{version}";
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://github.com/iqfareez/Chat-App-Project", UseShellExecute = true });
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://iqfareez.com", UseShellExecute = true });
        }
    }
}