using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Example
{
    class Program
    {
        private static string data;
        private static readonly object lockObject = new object();

        static void client_recver(int client_ticket, Socket client)
        {
            var ip = client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Client {client_ticket} : (From: {ip.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");

            var sb = new StringBuilder();
            var binary = new Byte[1024];

            var sendMsg = new byte[binary.Length];
            while (true)
            {
                lock (lockObject)
                {
                    if (data == null) continue;
                    sendMsg = Encoding.ASCII.GetBytes(data);
                }
                client.Send(sendMsg);
            }
        }

        static void Main(string[] args)
        {
            var ipep = new IPEndPoint(IPAddress.Any, 7777);
            using (Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                server.Bind(ipep);
                server.Listen(20);
                Console.WriteLine($"Server Start... Listen port {ipep.Port}...");

                List<Thread> clients = new List<Thread>();

                for (int i = 0; i < 2; i++)
                {
                    clients.Add(new Thread(() => client_recver(i, server.Accept())));
                    clients[i].Start();
                }
            
                while(true)
                {
                    Console.WriteLine("[new Order]");
                    lock (lockObject)
                    {
                        data = new string(Console.ReadLine());
                        if(data == null) break;
                    }
                }
            }
            Console.WriteLine("Press Any key...");
        }
    }
}
