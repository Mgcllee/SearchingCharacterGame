using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShortcutServer
{
    internal class Client
    {
        public Vector2 _position;
        public int dir {  get; set; }

        private Socket _socket{ get; set; }

        public Client(Socket socket, Vector2 position)
        {
            _socket = socket;
            _position = position;
        }

        public bool is_impact(Client other_client)
        {
            var dist = Math.Pow(_position.X - other_client._position.X, 2) + Math.Pow(_position.Y - other_client._position.Y, 2);
            if(dist <= 5000)
            {
                return true;
            }
            return false;
        }

        public void send_packet(string msg)
        {
            byte[] sendMsg = Encoding.ASCII.GetBytes(msg);
            if (sendMsg.Length > 0)
            {
                _socket.Send(sendMsg);
            }
        }
    }
}