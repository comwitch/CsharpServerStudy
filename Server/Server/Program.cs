using System;
using static System.Collections.Specialized.BitVector32;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;


namespace Server
{
    //tcp환경에서는 packet이 어떤 방식으로라도 완벽하게 도달했는지 알자줄 수 있는 장치가 필요하다.
    
    class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            //PacketManager.Instance.Register();

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Console.WriteLine("Listening");
            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });


            while (true)
            {
                Room.Push(() => Room.Flush());
                Thread.Sleep(250);
                
            }





        }
    }
}