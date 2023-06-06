using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Chat_Server
{
    internal enum User
    {
        Current,
        Other,
    }

    public partial class Form1 : Form
    {
        private MyTcpServer _server;
        private int _clientCount;
        private string _selectedImageFileName;

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
            // "[SERVER]" is message send by MyTcpServer class that is not
            // intended to be displayed to the user
            if (message.StartsWith("[SERVER"))
            {
                ProcessCommandFromServerBackend(message);
                return;
            }

            if (message.StartsWith("image;"))
            {
                // strip start
                message = message.Replace("image;", "");

                var fileFormat = MyFileUtility.GetFileExtension(message);

                // decode the image and save to temp directory
                string fileName = Guid.NewGuid() + $".{fileFormat}";
                fileName = Path.Combine(Path.GetTempPath(), fileName);
                File.WriteAllBytes(fileName, Convert.FromBase64String(message));

                // display the image in the Chat View
                Bitmap myBitmap = new Bitmap(fileName);
                AppendImageToChatView(myBitmap, User.Other);

                return;
            }

            // For normal message, append to the chat view
            AppendMessageToChatView(message.TrimEnd(), User.Other);
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            // ignore if message is empty
            if (string.IsNullOrEmpty(inputMessageTextbox.Text)) return;

            // Handle send image
            if (inputMessageTextbox.Text.StartsWith("[Image]"))
            {
                Image image = Image.FromFile(_selectedImageFileName);
                _server.SendImageMessage(_server.Clients.Last(), image, image.RawFormat);

                // add to chatView
                Bitmap myBitmap = new Bitmap(_selectedImageFileName);
                AppendImageToChatView(myBitmap, User.Current);

                inputMessageTextbox.Clear();
                return;
            }

            // Send message to last connected client
            // The requirement doesn't mention about handling multiple clients
            // so i think this implementation is OK
            AppendMessageToChatView(inputMessageTextbox.Text, User.Current);
            var clients = _server.Clients;
            _server.SendTextMessage(clients.Last(), inputMessageTextbox.Text);
            inputMessageTextbox.Clear();
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select image";
            fileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                // show only the filename to save textfield space
                inputMessageTextbox.Text = $"[Image] {fileDialog.SafeFileName}";
                _selectedImageFileName = fileDialog.FileName;
            }
        }

        private void ProcessCommandFromServerBackend(string message)
        {
            if (!message.Contains("[SERVER]")) throw new Exception("Invalid command");
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
            }
            else if (command.Contains("Client Disconnected"))
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
        }

        /// <summary>
        /// This component will be repeated in Client app, so remember
        /// to also reflect the changes there
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        private void AppendMessageToChatView(string message, User user)
        {
            var time = DateTime.Now.ToString("HH:mm");
            chatView.Invoke((MethodInvoker)(() => 
                    {
                        // differentiate looks between user (Client vs Server)
                        Color sendColor = user == User.Current ? Color.LightBlue : Color.Aqua;
                        chatView.SelectionAlignment =
                            user == User.Current ? HorizontalAlignment.Right : HorizontalAlignment.Left;

                        // Make a box chat bubble
                        // Basically a text with background highlight
                        // HACK: And with a little bit of 'padding' to the right
                        chatView.AppendText("  ");
                        chatView.SelectionFont = new Font(chatView.Font.FontFamily, 14, FontStyle.Regular);
                        chatView.SelectionBackColor = sendColor;
                        chatView.AppendText(" " + message + " ");

                        // Add time
                        chatView.SelectionBackColor = Color.Transparent;
                        chatView.SelectionFont = new Font(chatView.Font.FontFamily, 10, FontStyle.Italic);
                        chatView.AppendText($" {time}");

                        // HACK: Again, add 'padding' to the right
                        // apparently, using space character doesn't work so I had to use no colour character
                        chatView.SelectionColor = Color.Transparent;
                        chatView.AppendText("😺");

                        // and finally, break using newline
                        chatView.AppendText(Environment.NewLine);
                        chatView.AppendText(Environment.NewLine);
                    }
                ));
        }

        private void AppendImageToChatView(Bitmap bitmap, User user)
        {
            chatView.Invoke((MethodInvoker)(() =>
            {
                int maxWidth = 700;
                int maxHeight = 500;

                int newWidth, newHeight;
                double aspectRatio = (double)bitmap.Width / bitmap.Height;

                if (aspectRatio > 1)
                {
                    // Landscape image
                    newWidth = maxWidth;
                    newHeight = (int)(maxWidth / aspectRatio);
                }
                else
                {
                    // Portrait or square image
                    newHeight = maxHeight;
                    newWidth = (int)(maxHeight * aspectRatio);
                }

                Bitmap resized = new Bitmap(bitmap, new Size(newWidth, newHeight));
                Clipboard.SetDataObject(resized);
                DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);

                if (chatView.CanPaste(myFormat))
                {
                    chatView.SelectionAlignment =
                        user == User.Current ? HorizontalAlignment.Right : HorizontalAlignment.Left;

                    chatView.Paste(myFormat);
                }
            }));
            chatView.AppendText(Environment.NewLine);
            AppendMessageToChatView("Sent an image", user);
        }
    }
}