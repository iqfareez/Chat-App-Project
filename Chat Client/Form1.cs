using System;
using System.Net;
using System.Windows.Forms;

namespace Chat_Client
{
    public partial class Form1 : Form
    {
        private MyChatClient _chatClient = new MyChatClient();
        private bool isConnected = false;
        public Form1()
        {
            InitializeComponent();
            _chatClient.MessageReceived += OnMessageReceived;
            
            // disable the Chat View & Textbox before connecting to the server
            panel1.Enabled = false;
        }

        private void OnMessageReceived(object sender, string message)
        {
            richTextBox1.Invoke((MethodInvoker)(() =>
            {
                richTextBox1.AppendText("Server: " + message);
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ipAddress = IPAddress.Loopback.ToString();
            var port = 5050;

            // If not yet connected, then try to connect the server io
            if (!isConnected)
            {
                try
                {
                    _chatClient.Connect(ipAddress, port);
                    isConnected = true;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                panel1.Enabled = true;
                statusBar1.Text = $@"Connected to server at {ipAddress}:{port}";
                connectButton.Text = @"Disconnect";
            }
            else
            {
                // Disconnect from server
                _chatClient.Disconnect();
                statusBar1.Text = "Disconnected";
                connectButton.Text = "Connect";
                isConnected = false;
                panel1.Enabled = false;
            }
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            _chatClient.SendMessage(inputMessageTextbox.Text);
        }
    }
}