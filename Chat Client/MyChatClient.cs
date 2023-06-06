using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Client
{
    public class MyChatClient
    {
        private TcpClient _client;

        public event EventHandler<string> MessageReceived;

        /// <summary>
        /// Connect to the TCP server
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public void Connect(string ipAddress, int port)
        {
            _client = new TcpClient();
            _client.Connect(ipAddress, port);

            // Start a separate task to receive data
            _ = ReceiveDataAsync();
        }

        /// <summary>
        /// Send message to the connected server
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            NetworkStream stream = _client.GetStream();
            byte[] buffer = Encoding.ASCII.GetBytes(message + Environment.NewLine);
            stream.Write(buffer, 0, buffer.Length);
        }
        
        public void SendImageMessage(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                
                StringBuilder sb = new StringBuilder();
                sb.Append("image;");
                sb.Append(base64String);

                NetworkStream stream = _client.GetStream();
                byte[] buffer = Encoding.ASCII.GetBytes(sb + Environment.NewLine);
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Handle incoming data from the server
        /// </summary>
        private async Task ReceiveDataAsync()
        {
            NetworkStream stream = _client.GetStream();
            byte[] buffer = new byte[1024];
            StringBuilder messageBuilder = new StringBuilder();

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    break; // Server or client disconnected
                }

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                messageBuilder.Append(data);

                if (data.EndsWith("\n")) // Check if received data ends with a newline
                {
                    string receivedMessage = messageBuilder.ToString();
                    OnMessageReceived(receivedMessage); // Remove the newline character before triggering the event
                    messageBuilder.Clear();
                }
            }

            _client.Close();
        }

        public void Disconnect()
        {
            _client?.Close();
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }
    }
}
