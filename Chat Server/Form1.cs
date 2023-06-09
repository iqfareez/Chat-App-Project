using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Chat_Server
{
    public enum User
    {
        Current,
        Other,
    }

    public partial class Form1 : Form
    {
        private MyTcpServer _server;
        private int _clientCount;
        private string _selectedImageFileName;
        private MyHistoryManager _myHistoryManager = new MyHistoryManager();
        private Timer timer;

        public Form1()
        {
            InitializeComponent();

            // disable Chat View & Textbox false before starting the server
            panel1.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set up the timer control
            timer = new Timer();
            timer.Interval = 3000; // 3 seconds
            timer.Tick += Timer_Tick;
            
            // Hardcode the IP address and port for now
            // The client will be running on the same machine as the client
            _server = new MyTcpServer(IPAddress.Loopback.ToString(), 5050);

            // subscribe to the MessageReceived event
            _server.MessageReceived += OnMessageReceived;

            // Start the server
            _ = _server.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            timer.Stop();
        
            // Restore the original text
            statusBar1.Text = "";
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
                
                _myHistoryManager.AddMessage($"[Image] Sent an image", User.Other);
                
                chatView.ScrollToCaret();

                return;
            }

            // For normal message, append to the chat view
            AppendMessageToChatView(message.TrimEnd(), User.Other);
            _myHistoryManager.AddMessage(message.TrimEnd(), User.Other);
            chatView.ScrollToCaret();
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
                
                _myHistoryManager.AddMessage($"[Image] Sent an image", User.Current);
                
                chatView.ScrollToCaret();

                // restore the editing capability of the textbox
                inputMessageTextbox.ReadOnly = false;
                return;
            }

            // Send message to last connected client
            // The requirement doesn't mention about handling multiple clients
            // so i think this implementation is OK
            AppendMessageToChatView(inputMessageTextbox.Text, User.Current);
            _myHistoryManager.AddMessage(inputMessageTextbox.Text, User.Current);
            var client = _server.Clients.Last();
            _server.SendTextMessage(client, inputMessageTextbox.Text);
            
            chatView.ScrollToCaret();
            
            // clear the send message box
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
                // prevent user form modifying the text input - just a UX thingy
                inputMessageTextbox.ReadOnly = true;
                
                // record the selected image file path
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
                        chatView.SelectionFont = new Font(chatView.Font.FontFamily, 12, FontStyle.Regular);
                        chatView.SelectionBackColor = sendColor;
                        chatView.AppendText(" " + message + " ");

                        // Add time
                        chatView.SelectionBackColor = Color.Transparent;
                        chatView.SelectionFont = new Font(chatView.Font.FontFamily, 9, FontStyle.Italic);
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
                int maxWidth = 500;
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
                
                chatView.ReadOnly = false; // enable editing to allow image pasting
                
                // move the caret to the end of the text
                chatView.Select(chatView.Text.Length, 0);

                if (chatView.CanPaste(myFormat))
                {
                    chatView.SelectionAlignment =
                        user == User.Current ? HorizontalAlignment.Right : HorizontalAlignment.Left;

                    chatView.Paste(myFormat);
                }
                
                // disable editing again
                chatView.ReadOnly = true;
            }));
            chatView.AppendText(Environment.NewLine);
            AppendMessageToChatView("Sent an image", user);
        }

        private void inputMessageTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                sendMessageButton.PerformClick();
            }
        }

        private void exportFileHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currentDateTimestamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm");
            var data = _myHistoryManager.GetExportHistory();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text File (*.txt)|*.txt";
            saveFileDialog.Title = "Save chat history";
            saveFileDialog.FileName = $"ChatHistory-{currentDateTimestamp}.txt";
            saveFileDialog.ShowDialog();
            
            if (saveFileDialog.FileName != "")
            {
                File.WriteAllText(saveFileDialog.FileName, data);
                statusBar1.Text = $"Chat history exported to {saveFileDialog.FileName}";
                timer.Start();
            }
        }
    }
}