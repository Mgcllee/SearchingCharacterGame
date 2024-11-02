using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Example
{
    class Program
    {
        // 실행 함수
        static void Main(string[] args)
        {
            // Socket EndPoint 설정(서버의 경우는 Any로 설정하고 포트 번호만 설정한다.)
            var ipep = new IPEndPoint(IPAddress.Any, 7777);
            // 소켓 인스턴스 생성
            using (Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // 서버 소켓에 EndPoint 설정
                server.Bind(ipep);
                // 클라이언트 소켓 대기 버퍼
                server.Listen(20);
                // 콘솔 출력
                Console.WriteLine($"Server Start... Listen port {ipep.Port}...");
                // 클라이언트로부터 접속 대기
                using (var client = server.Accept())
                {
                    var ip = client.RemoteEndPoint as IPEndPoint;
                    Console.WriteLine($"Client : (From: {ip.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");
                    
                    var sb = new StringBuilder();
                    // 통신 바이너리 버퍼
                    var binary = new Byte[1024];
                    // 무한 루프
                    while (true)
                    {
                        string data = Console.ReadLine();
                        if (data == null) break;
                        var sendMsg = Encoding.ASCII.GetBytes(data);
                        client.Send(sendMsg);
                    }
                }
            }
            // 아무 키나 누르면 종료
            Console.WriteLine("Press Any key...");
        }
    }
}
