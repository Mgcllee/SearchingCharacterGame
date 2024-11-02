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
            var ipep = new IPEndPoint(IPAddress.Any, 10000);
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
                    // 클라이언트 EndPoint 정보 취득
                    var ip = client.RemoteEndPoint as IPEndPoint;
                    // 콘솔 출력 - 접속 ip와 접속 시간
                    Console.WriteLine($"Client : (From: {ip.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");
                    // 클라이언트로 접속 메시지를 byte로 변환하여 송신
                    client.Send(Encoding.ASCII.GetBytes("Welcome server!\r\n>"));
                    // 메시지 버퍼
                    var sb = new StringBuilder();
                    // 통신 바이너리 버퍼
                    var binary = new Byte[1024];
                    // 무한 루프
                    while (true)
                    {
                        // 클라이언트로부터 메시지 대기
                        client.Receive(binary);
                        // 클라이언트로 받은 메시지를 String으로 변환
                        var data = Encoding.ASCII.GetString(binary);
                        // 메시지 공백(\0)을 제거
                        sb.Append(data.Trim('\0'));
                        // 메시지 총 내용이 2글자 이상이고 개행(\r\n)이 발생하면
                        if (sb.Length > 2 && sb[sb.Length - 2] == '\r' && sb[sb.Length - 1] == '\n')
                        {
                            // 메시지 버퍼의 내용을 String으로 변환
                            data = sb.ToString().Replace("\n", "").Replace("\r", "");
                            // 메시지 내용이 공백이라면 계속 메시지 대기 상태로
                            if (String.IsNullOrWhiteSpace(data))
                            {
                                continue;
                            }
                            // 메시지 내용이 exit라면 무한 루프 종료(즉, 서버 종료)
                            if ("EXIT".Equals(data, StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                            // 메시지 내용을 콘솔에 표시
                            Console.WriteLine("Message = " + data);
                            // 버퍼 초기화
                            sb.Length = 0;
                            // 메시지에 ECHO를 붙힘
                            var sendMsg = Encoding.ASCII.GetBytes("ECHO : " + data + "\r\n>");
                            // 클라이언트로 메시지 송신
                            client.Send(sendMsg);
                        }
                    }
                    // 콘솔 출력 - 접속 종료 메시지
                    Console.WriteLine($"Disconnected : (From: {ip.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");
                }
            }
            // 아무 키나 누르면 종료
            Console.WriteLine("Press Any key...");
            Console.ReadLine();
        }
    }
}
