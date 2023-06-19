using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Chat_Client
{
    public enum User
    {
        Current,
        Other,
    }

    public partial class Form1 : Form
    {
        private readonly MyChatClient _chatClient = new MyChatClient();
        private MyHistoryManager _historyManager = new MyHistoryManager();
        private string _selectedImageFileName;

        private bool _isConnected = false;

        // private string messageReceived;
        public Form1()
        {
            InitializeComponent();
            _chatClient.MessageReceived += OnMessageReceived;

            // disable the Chat View & Textbox before connecting to the server
            panel1.Enabled = false;
        }

        private void OnMessageReceived(object sender, string message)
        {
            if (message.StartsWith("image;"))
            {
                // strip start to get the base64 string only
                message = message.Replace("image;", "");

                // Convert base64 to Image object. Ref: https://stackoverflow.com/a/21325711/13617136
                var img = Image.FromStream(new MemoryStream(Convert.FromBase64String(message)));
                Bitmap imgBitmap = new Bitmap(img);

                // display the image in the Chat View
                AppendImageToChatView(imgBitmap, User.Other);

                // record the image in the history
                _historyManager.AddMessage($"[Image] Sent an image", User.Other);
                
                chatView.ScrollToCaret();
                return;
            }
            
            AppendMessageToChatView(message.TrimEnd(), User.Other);
            chatView.ScrollToCaret();
            _historyManager.AddMessage(message.TrimEnd(), User.Other);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ipAddress = IPAddress.Loopback.ToString();
            var port = 5050;

            // If not yet connected, then try to connect the server io
            if (!_isConnected)
            {
                try
                {
                    _chatClient.Connect(ipAddress, port);
                    _isConnected = true;
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
                _isConnected = false;
                panel1.Enabled = false;
            }
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            // ignore if message is empty
            if (string.IsNullOrEmpty(inputMessageTextbox.Text)) return;

            // Handle send image
            if (inputMessageTextbox.Text.StartsWith("[Image]") && _selectedImageFileName != null)
            {
                Image image = Image.FromFile(_selectedImageFileName);
                _chatClient.SendImageMessage(image, image.RawFormat);

                // add to chatView
                Bitmap myBitmap = new Bitmap(_selectedImageFileName);
                AppendImageToChatView(myBitmap, User.Current);
                
                // restore editing capability of the textbox
                inputMessageTextbox.ReadOnly = false;
                
                // reset the button
                openFileButton.BackgroundImage = Properties.Resources.attachment_icon;
                _selectedImageFileName = null;
            }
            else
            {
                // handle normal text message
                _chatClient.SendMessage(inputMessageTextbox.Text);
                AppendMessageToChatView(inputMessageTextbox.Text, User.Current);
            }

            _historyManager.AddMessage(inputMessageTextbox.Text, User.Current);
            chatView.ScrollToCaret();
            inputMessageTextbox.Clear();
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            // check if the button is already in 'cancel' mode
            if (_selectedImageFileName != null)
            {
                _selectedImageFileName = null;
                openFileButton.BackgroundImage = Properties.Resources.attachment_icon;
                
                // openFileButton.BackgroundImageLayout = ImageLayout.Zoom;
                inputMessageTextbox.Text = "";
                inputMessageTextbox.ReadOnly = false;
                return;
            }
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select image";
            fileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                // change the icon to cancel button
                openFileButton.BackgroundImage = Properties.Resources.cancel_icon;
                // show only the filename to save textfield space
                inputMessageTextbox.Text = $"[Image] {fileDialog.SafeFileName}";
                inputMessageTextbox.ReadOnly = true;
                _selectedImageFileName = fileDialog.FileName;
            }
        }

        private void AppendMessageToChatView(string message, User user)
        {
            var time = DateTime.Now.ToString("HH:mm");
            chatView.Invoke((MethodInvoker)(() =>
                    {
                        // differentiate looks between user (Client vs Server)
                        Color sendColor = user == User.Current ? Color.Bisque : Color.Pink;
                        chatView.SelectionAlignment =
                            user == User.Current ? HorizontalAlignment.Right : HorizontalAlignment.Left;

                        // Make a box chat bubble
                        // Basically a text with background highlight
                        chatView.AppendText("  ");
                        chatView.SelectionFont = new Font(chatView.Font.FontFamily, 12, FontStyle.Regular);
                        chatView.SelectionBackColor = sendColor;
                        chatView.AppendText(" " + message + " ");

                        // Add time
                        chatView.SelectionBackColor = Color.Transparent;
                        chatView.SelectionFont = new Font(chatView.Font.FontFamily, 9, FontStyle.Italic);
                        chatView.AppendText($" {time}");

                        // Again, add 'padding' to the right
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
                // chatView.AppendText(message + Environment.NewLine);
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
                
                // by default, the chatview is in read-only mode
                // but, to append image (by pasting it), we need to disable it first
                chatView.ReadOnly = false;
                
                // move the caret to the end of the text
                chatView.Select(chatView.Text.Length, 0);

                if (chatView.CanPaste(myFormat))
                {
                    chatView.SelectionAlignment =
                        user == User.Current ? HorizontalAlignment.Right : HorizontalAlignment.Left;

                    chatView.Paste(myFormat);
                }
                
                // restore the read-only mode
                chatView.ReadOnly = true;

                chatView.AppendText(Environment.NewLine);
                AppendMessageToChatView("Sent an image", user);
            }));
        }


        private void inputMessageTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            // Enter key to send message
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // disable 'ding' sound
                sendMessageButton.PerformClick(); // 'Click' the send button
            }
        }

        private void exportChatHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var currentDateTimestamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm");
            var data = _historyManager.GetExportHistory();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text File (*.txt)|*.txt";
            saveFileDialog.Title = "Save chat history";
            saveFileDialog.FileName = $"ChatHistory-{currentDateTimestamp}.txt";
            saveFileDialog.ShowDialog();
            
            if (saveFileDialog.FileName != "")
            {
                File.WriteAllText(saveFileDialog.FileName, data);
            }
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show info dialog
            var infoDialog = new FormAbout();
            infoDialog.ShowDialog();
        }
    }
}