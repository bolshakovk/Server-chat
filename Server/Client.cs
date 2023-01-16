using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private string _userName;
        private readonly Socket _handler;
        private readonly Thread _userThread;
        
        // В конструкторе инициализируем поток и запускаем поток для клиента и привязываем к сокету
        public Client(Socket socket)
        {
            _handler = socket;
            _userThread = new Thread(Listener)
            {
                IsBackground = true
            };
            _userThread.Start();
        }

        // геттер
        public string UserName
        {
            get { return _userName; }
        }

        private void Listener()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int byteRec = _handler.Receive(buffer);
                    string data = Encoding.UTF8.GetString(buffer, 0, byteRec);
                    HandleCommand(data);
                }
                catch { Server.EndClient(this); return; }

            }
        }
        // Закрытие сокета и потока
        public void End()
        {
            try
            {
                _handler.Close();
                try
                {
                    _userThread.Abort();
                }
                catch { }

            }
            catch (Exception exp) { Console.WriteLine("error with end: {0}", exp.Message); }

        }
        // Считывание командной строки и обновление окна чата или добавление сообщения
        private void HandleCommand(string data)
        {
            if (data.Contains("#setname"))
            {
                _userName = data.Split('&')[1];
                UpdateChat();
                return;
            }
            if (data.Contains("#newmsg"))
            {
                string message = data.Split('&')[1];
                ChatController.AddMessage(_userName, message);
                return;
            }
        }
        // Отправить обновленный чат
        public void UpdateChat()
        {
            Send(ChatController.GetChat());
        }
        // Передача статуса в консольную команду
        public void Send(string command)
        {
            try
            {
                int byteSent = _handler.Send(Encoding.UTF8.GetBytes(command));
                if (byteSent > 0) Console.WriteLine("Succsess");
            }
            catch (Exception exp) { Console.WriteLine("Error with send command: {0}.", exp.Message); Server.EndClient(this); }
        }
    }
}
