using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Chat_Server
{
    public partial class Form1 : Form
    {
        private MyTcpServer _server;
        private int _clientCount;

        public Form1()
        {
            InitializeComponent();
            
            // disable Chat View & Textbox false before starting the server
            panel1.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Hardcode the IP address and port for now
            // The client will be running on the same machine as the client
            _server = new MyTcpServer(IPAddress.Loopback.ToString(), 5050);
            
            // subscribe to the MessageReceived event
            _server.MessageReceived += OnMessageReceived;

            // Start the server
            _ = _server.Start();
        }

        private void OnMessageReceived(object sender, string message)
        {
            // Check if the message is special message (from backend to the UI)
            // It will start with "[SERVER]"
            if (message.StartsWith("[SERVER"))
            {
                // extract the 'command'
                var command = message.Replace("[SERVER]", "").Trim();

                // Handle commands
                if (command == "Started")
                {
                    statusLabel.Text = "Server Online. Awaiting connection...";
                }
                else if (command.Contains("Client Connected"))
                {
                    panel1.Enabled = true;
                    statusLabel.BackColor = Color.Green;
                    _clientCount++;
                    statusLabel.Text = "Server Online. Connected";
                } else if (command.Contains("Client Disconnected"))
                {
                    _clientCount--;
                    if (_clientCount == 0)
                    {
                        statusLabel.BackColor = Color.Orange;
                        panel1.Enabled = false;
                        statusLabel.Text = "Server Online. Awaiting connection...";
                    }
                    else
                    {
                        statusLabel.Text = $"Server Connected";
                    }
                }
                
                // Special message are not going to be displayed on the console
                return;
            }

            // Add message to the Chat View
            chatView.Invoke((MethodInvoker)(() => { chatView.AppendText(message + Environment.NewLine); }));
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            // ignore if message is empty
            if (string.IsNullOrEmpty(inputMessageTextbox.Text)) return;

            // Send message to last connected client
            // The requirement doesn't mention about handling multiple clients
            // so i think this implementation is OK
            var clients = _server.Clients;
            _server.SendMessage(clients.Last(), inputMessageTextbox.Text);
        }
    }
}