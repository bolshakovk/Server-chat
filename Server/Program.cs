using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private const string _host = "localhost";
        private const int _serverPort = 9933;
        private static Thread _serverThread;

        static void Main(string[] args)
        {
            // открыть поток для сервера
            _serverThread = new Thread(StartServer)
            {
                IsBackground = true
            };
            _serverThread.Start();
            // считывание команд во время работы
            while (true)
                HandlerCommands(Console.ReadLine());

        }
        // считывание команд для каждого пользователя
        private static void HandlerCommands(string cmd)
        {
            cmd = cmd.ToLower();
            if (cmd.Contains("/getusers"))
            {
                int countUsers = Server.Clients.Count;
                for (int i = 0; i < countUsers; i++)
                {
                    Console.WriteLine("[{0}]: {1}", i, Server.Clients[i].UserName);
                }
            }
        }
        // Открыть порт, хост соединение, привязать к сокету
        private static void StartServer()
        {
            IPHostEntry iPHost = Dns.GetHostEntry(_host);
            IPAddress iPAddress = iPHost.AddressList[0];
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, _serverPort);
            Socket socket = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(iPEndPoint);
            socket.Listen(1000);
            Console.WriteLine("Server has been started on IP: {0}", iPEndPoint);
            while (true)
            {
                try
                {
                    Socket user = socket.Accept();
                    Server.NewClient(user);
                }
                catch (Exception exp) { Console.WriteLine("Error: {0}", exp.Message); }

            }
        }
    }
}
