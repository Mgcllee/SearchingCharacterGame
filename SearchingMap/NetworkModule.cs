using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SearchingMap
{
    class NetworkModule
    {
        private Vector2 _position;
        public Vector2 get_position()
        {
            return _position;
        }

        // 실행 함수
        public void connect_server()
        {
            // Socket EndPoint 설정
            var ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
            // 소켓 인스턴스 생성
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // 소켓 접속
                client.Connect(ipep);
                // 접속이 되면 Task로 병렬 처리
                new Task(() =>
                {
                    try
                    {
                        while (true)
                        {
                            var binary = new Byte[1024];
                            // 서버로부터 메시지 대기
                            client.Receive(binary);
                            var data = Encoding.ASCII.GetString(binary).Trim('\0');
                            // 메시지 내용이 공백이라면 계속 메시지 대기 상태로
                            if (String.IsNullOrWhiteSpace(data))
                            {
                                continue;
                            }

                            string[] pos = data.Split(" ");

                            _position.X = int.Parse(pos[0]);
                            _position.Y = int.Parse(pos[1]);
                        }
                    }
                    catch (SocketException)
                    {
                        // 접속 끝김이 발생하면 Exception이 발생
                    }
                    // Task 실행
                }).Start();
            }
        }
    }
}
