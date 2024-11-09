using ShortcutServer;
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
        private static List<Client> clients = new List<Client>();
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

        static bool check_collision(int client_ticket)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (i == client_ticket) continue;
                lock (lockObject)
                {
                    if (clients[i].is_impact(clients[client_ticket]))
                    {
                        Console.WriteLine($"Client ({client_ticket}) and ({i}) impact!!!");
                        return true;
                    }
                }
            }
            return false;
        }

        static void move_client(int client_ticket)
        {
            Console.WriteLine($"Client {client_ticket} : Connection time: {DateTime.Now}");
            int[] dir_row = new int[8] { -1, -1, 0, 1, 1, 1, 0, -1 };
            int[] dir_col = new int[8] { 0, 1, 1, 1, 0, -1, -1, -1 };

            while (true)
            {
                Thread.Sleep(20);

                lock (lockObject)
                {
                    int dir = clients[client_ticket].dir;
                    float new_X = clients[client_ticket]._position.X + dir_col[dir] * 2.0f;
                    float new_Y = clients[client_ticket]._position.Y + dir_row[dir] * 2.0f;

                    bool is_impact = check_collision(client_ticket);
                    if (0 < new_X && new_X + 100 < 800 && 0 < new_Y && new_Y + 100 < 500
                        && false == is_impact)
                    {
                        clients[client_ticket]._position.X = new_X;
                        clients[client_ticket]._position.Y = new_Y;
                    }
                    else if(is_impact)
                    {
                        clients[client_ticket].dir = (clients[client_ticket].dir + 4) % 8;
                        clients[client_ticket]._position.X += dir_col[clients[client_ticket].dir] * 4.0f;
                        clients[client_ticket]._position.Y += dir_row[clients[client_ticket].dir] * 4.0f;
                    } 
                    else
                    {
                        clients[client_ticket].dir = random.Next(0, 8);
                    }
                }

                string data = $"{client_ticket} {clients[client_ticket]._position.X} {clients[client_ticket]._position.Y}";
                foreach (var client in clients)
                {
                    client.send_packet(data);
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

                List<Thread> client_threads = new List<Thread>();

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
                            Vector2 vector2 = new Vector2(random.Next(10, 700), random.Next(10, 400));
                            Client new_client = new Client(new_client_socket, vector2);
                            clients.Add(new_client);
                            break;
                        }
                    }
                    Console.WriteLine($"Client {user_ticket} connected.");
                    client_threads.Add(new Thread(() => move_client(user_ticket)));
                    client_threads[i].Start();
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
