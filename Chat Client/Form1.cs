using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Chat_Client
{
    internal enum User
    {
        Current,
        Other,
    }

    public partial class Form1 : Form
    {
        private MyChatClient _chatClient = new MyChatClient();
        private string _selectedImageFileName;

        private bool isConnected = false;

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

            AppendMessageToChatView(message.TrimEnd(), User.Other);
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
            // ignore if message is empty
            if (string.IsNullOrEmpty(inputMessageTextbox.Text)) return;

            // Handle send image
            if (inputMessageTextbox.Text.StartsWith("[Image]"))
            {
                Image image = Image.FromFile(_selectedImageFileName);
                _chatClient.SendImageMessage(image, image.RawFormat);

                // add to chatView
                Bitmap myBitmap = new Bitmap(_selectedImageFileName);
                AppendImageToChatView(myBitmap, User.Current);

                inputMessageTextbox.Clear();
                
                chatView.ScrollToCaret();
                
                // restore editing capability of the textbox
                inputMessageTextbox.ReadOnly = false;
                return;
            }

            AppendMessageToChatView(inputMessageTextbox.Text, User.Current);
            _chatClient.SendMessage(inputMessageTextbox.Text);
            chatView.ScrollToCaret();
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
    }
}