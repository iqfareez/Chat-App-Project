using System;
using System.Collections.Generic;
using System.Text;

namespace Chat_Client
{
    /// <summary>
    /// Manage chat history such as adding and exporting
    /// for the client
    /// </summary>
    public class MyHistoryManager
    {
        /// <summary>
        /// Record every message added with its metadata
        /// </summary>
        private List<MessageItem> _messages = new List<MessageItem>();
        
        /// <summary>
        /// Add a message to the history
        /// </summary>
        /// <param name="message">The actual message</param>
        /// <param name="user">Message send from</param>
        public void AddMessage(string message, User user)
        {
            MessageItem item = new MessageItem
            {
                Message = message,
                Time = DateTime.Now,
                User = user
            };
            _messages.Add(item);
        }
        
        /// <summary>
        /// Generate a string of chat history to be exported
        /// </summary>
        /// <returns></returns>
        public string GetExportHistory()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in _messages)
            {
                builder.AppendLine(item.ToHistoryItemString());
            }

            return builder.ToString();
        }
        
        private class MessageItem
        {
            public string Message;
            public DateTime Time;
            public User User;

            public string ToHistoryItemString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("[");
                builder.Append(User == User.Current? "Client" : "Server");
                builder.Append(" ");
                builder.Append(Time.ToString("HH:mm:ss"));
                builder.Append("] ");
                builder.Append(Message);
                return builder.ToString();
            }
        }
        
    }

    
}