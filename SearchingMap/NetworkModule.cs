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
        private Socket client;
        private Vector2 _position;
        public Vector2 get_position()
        {
            return _position;
        }

        public NetworkModule()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        // 실행 함수
        public void connect_server()
        {
            client.Connect("127.0.0.1", 7777);
        }

        public bool recive_position()
        {
            var binary = new Byte[1024];
            int bytesReceived = client.Receive(binary);

            if (bytesReceived == 0) return false;

            var data = Encoding.ASCII.GetString(binary, 0, bytesReceived);

            string[] pos = data.Split(" ");

            _position.X = int.Parse(pos[0]);
            _position.Y = int.Parse(pos[1]);

            return true;
        }
    }
}
