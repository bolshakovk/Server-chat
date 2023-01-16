using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class ChatController
    {
        private const int _maxMessage = 100;
        public static List<Message> Chat = new List<Message>();
        public struct Message
        {
            public string userName;
            public string data;
            public Message(string name, string msg)
            {
                userName = name;
                data = msg;
            }
        }
        public static void AddMessage(string userName, string msg)
        {
            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(msg)) return;
                int countMessages = Chat.Count;
                if (countMessages > _maxMessage) ClearChat();
                Message newMessage = new Message(userName, msg);
                Chat.Add(newMessage);
                Console.WriteLine("New message from {0}.", userName);
                Server.UpdateAllChats();
            }
            catch (Exception exp) { Console.WriteLine("Error with addMessage: {0}.", exp.Message); }


        }
        public static void ClearChat()
        {
            Chat.Clear();
        }
        public static string GetChat()
        {
            try
            {
                string data = "#updatechat&";
                int countMessages = Chat.Count;
                if (countMessages <= 0) return string.Empty;
                for (int i = 0; i < countMessages; i++)
                {
                    data += String.Format("{0}~{1}|", Chat[i].userName, Chat[i].data);
                }
                return data;
            }
            catch (Exception exp) { Console.WriteLine("Error with getChat: {0}", exp.Message); return string.Empty; }
        }
    }
}
