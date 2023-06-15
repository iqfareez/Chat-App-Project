using System;
using System.Windows.Forms;

namespace Chat_Server
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            // just an easter egg
            MessageBox.Show("Software Engineering is awesome!!");
        }
    }
}