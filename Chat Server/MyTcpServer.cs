﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat_Server
{
    public class MyTcpServer
    {
        private TcpListener _server;
        private List<TcpClient> _clients = new List<TcpClient>();
        private readonly string _ipAddress;
        private readonly int _port;

        public event EventHandler<string> MessageReceived;

        public int Port => _port;
        public List<TcpClient> Clients => _clients;

        /// <summary>
        /// TCP Server helper class
        /// </summary>
        /// <param name="ipAddress">Default at loopback IP (Cannot be accessed from the outside world)</param>
        /// <param name="port"></param>
        public MyTcpServer(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public async Task Start()
        {
            // Abort operation if server is already running
            if (_server != null)
            {
                throw new InvalidOperationException("Server is already running!");
            }

            IPAddress localAddr = IPAddress.Parse(_ipAddress);
            _server = new TcpListener(localAddr, _port);
            _server.Start();

            OnMessageReceived("[SERVER] Started");

            while (true)
            {
                // Handle new client connection
                TcpClient client = await _server.AcceptTcpClientAsync();
                _clients.Add(client);
                OnMessageReceived($"[SERVER] Client Connected. IP: {client.Client.RemoteEndPoint}");
                _ = HandleClientAsync(client);
            }
        }

        public void Stop()
        {
            _server?.Stop();
            _server = null;
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            StringBuilder messageBuilder = new StringBuilder();

            // Receive data in a loop until the client disconnects
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    OnMessageReceived($"[SERVER] Client Disconnected. IP: {client.Client.RemoteEndPoint}");
                    break; // Client disconnected
                }

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                messageBuilder.Append(data);

                if (data.Length > 0)
                {
                    string receivedMessage = messageBuilder.ToString().TrimEnd();

                    // Raise the MessageReceived event
                    OnMessageReceived($"{client.Client.RemoteEndPoint}: {receivedMessage}");

                    byte[] response = Encoding.ASCII.GetBytes(receivedMessage.ToUpper() + Environment.NewLine);
                    await stream.WriteAsync(response, 0, response.Length);

                    messageBuilder.Clear();
                }
            }
        }

        /// <summary>
        /// Send message to the UI thread
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }
        
        /// <summary>
        /// Send message to the connected clients
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        public void SendMessage(TcpClient client, string message)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                // show error MessageBox
                MessageBox.Show("Failed to send message. Perhaps the client has been disconnected?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        
        
    }
}
