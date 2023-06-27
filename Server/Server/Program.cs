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
    class Knight
    {
        public int hp;
        public int attack;
        public string name;
        public List<int> skills = new List<int>();
    }
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Onconnected :  {endPoint} ");

            //보낸다
            Knight kight = new Knight() { hp = 100, attack = 10 };
            
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(kight.hp);
            byte[] buffer2 = BitConverter.GetBytes(kight.attack);
            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            ArraySegment<byte> sedBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);


            Send(sendBuff);

            Thread.Sleep(1000);

            Disconnect();
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"Transferred bytes :  {endPoint} ");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($" [From Client] {recvData}");

            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes :  {numOfBytes} ");

        }
    }

    class Program
    {
        static Listener _listener = new Listener();



        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Console.WriteLine("Listening");
            _listener.Init(endPoint, () => { return new GameSession(); });


            while (true)
            {


            }





        }
    }
}