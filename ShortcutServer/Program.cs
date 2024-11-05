using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace Example
{
    class Program
    {
        private static string data = "";
        private static readonly object lockObject = new object();
        private static List<Socket> clients_socket = new List<Socket> ();

        static void client_recver(int client_ticket)
        {
            Console.WriteLine($"Client {client_ticket} : Connection time: {DateTime.Now})");

            Vector2 _position = new Vector2();
            _position.X = 10;
            _position.Y = 10;

            int dir = 0;
            int[] dir_row = new int[8] { -1, -1, 0, 1, 1, 1, 0, -1 };
            int[] dir_col = new int[8] { 0, 1, 1, 1, 0, -1, -1, -1 };

            var sb = new StringBuilder();
            var binary = new Byte[1024];

            var sendMsg = new byte[binary.Length];
            while (true)
            {
                lock (lockObject)
                {
                    // if (data == null)
                    {
                        Thread.Sleep(30);

                        float new_X = _position.X + dir_col[dir] * 4.0f;
                        float new_Y = _position.Y + dir_row[dir] * 4.0f;

                        if (0 < new_X && new_X + 100 < 800
                            && 0 < new_Y && new_Y + 100 < 500)
                        {
                            _position.X = new_X;
                            _position.Y = new_Y;
                        }
                        else
                        {
                            dir = new Random().Next() % 8;
                        }

                        data = client_ticket.ToString();
                        data += " ";
                        data += _position.X.ToString();
                        data += " ";
                        data += _position.Y.ToString();
                        sendMsg = Encoding.ASCII.GetBytes(data);

                        foreach (var socket in clients_socket)
                        {
                            if(sendMsg != null && SocketConnected(socket))
                            {
                                socket.Send(sendMsg);
                            }
                        }
                        // client.Send(sendMsg);
                    }
                }
            }
        }

        static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
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
                    Socket socket = null;
                    while(true)
                    {
                        socket = server.Accept();
                        if(SocketConnected(socket) && socket != null) break;
                    }
                    clients_socket.Add(socket);
                    clients.Add(new Thread(() => client_recver(i)));
                    clients[i].Start();
                }
                
                while(true)
                {
                    Console.WriteLine($"Server Running...");
                }
            }
        }
    }
}
