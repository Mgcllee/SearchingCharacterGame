using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Example
{
    class Program
    {
        private static readonly object lockObject = new object();
        private static List<Socket> clients_socket = new List<Socket>();
        private static Random random = new Random(); // Random 객체를 클래스 수준에서 생성

        static private async Task print_data(string data)
        {
            // 비동기 람다 함수 정의
            Func<Task> asyncLambda = async () =>
            {
                await Task.Delay(2000); // 2초 대기
                Console.WriteLine(data);
            };

            // 비동기 람다 함수 호출
            await asyncLambda();
        }

        static void client_recver(int client_ticket)
        {
            Console.WriteLine($"Client {client_ticket} : Connection time: {DateTime.Now}");

            Vector2 _position = new Vector2(10, 10);
            int dir = 0;
            int[] dir_row = new int[8] { -1, -1, 0, 1, 1, 1, 0, -1 };
            int[] dir_col = new int[8] { 0, 1, 1, 1, 0, -1, -1, -1 };

            while (true)
            {
                Thread.Sleep(100);

                float new_X = _position.X + dir_col[dir] * 2.0f;
                float new_Y = _position.Y + dir_row[dir] * 2.0f;

                if (0 < new_X && new_X + 100 < 800 && 0 < new_Y && new_Y + 100 < 500)
                {
                    _position.X = new_X;
                    _position.Y = new_Y;
                }
                else
                {
                    dir = random.Next(0, 8);
                }

                string data = $"{client_ticket} {_position.X} {_position.Y}";

                byte[] sendMsg = Encoding.ASCII.GetBytes(data);
                if (sendMsg.Length > 0)
                {
                    lock (lockObject)
                    {
                        for (int i = 0; i < clients_socket.Count; i++)
                        {
                            if (clients_socket[i] != null && clients_socket[i].Connected)
                            {
                                try
                                {
                                    if(_position.X % 10 == 0 && _position.Y % 10 == 0)
                                    {
                                        print_data(data);
                                    }
                                    clients_socket[i].Send(sendMsg);
                                }
                                catch (SocketException ex)
                                {
                                    Console.WriteLine($"Error sending to client {i}: {ex.Message}");
                                    return;
                                }
                            }
                        }
                    }
                }
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
                    int user_ticket = i;
                    Socket new_client_socket = null;

                    Console.WriteLine($"wait for new client...");
                    while (true)
                    {
                        new_client_socket = server.Accept();
                        if (new_client_socket.Connected)
                        {
                            lock (lockObject)
                            {
                                clients_socket.Add(new_client_socket);
                            }
                            break;
                        }
                    }
                    Console.WriteLine($"Client {user_ticket} connected.");
                    clients.Add(new Thread(() => client_recver(user_ticket)));
                    clients[i].Start();
                }

                Console.WriteLine($"Server Running...");
                while (true)
                {
                    // 서버가 계속 실행되도록 대기
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
