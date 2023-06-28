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
    class Packet
    {
        public ushort size; //packet size
        public ushort packetId; //identity
    }


    class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Onconnected :  {endPoint} ");

            //보낸다
            //Packet packet = new Packet() { size = 100, packetId = 10 };
            
            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);


            //Send(sendBuff);

            Thread.Sleep(5000);

            Disconnect();
            //Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"Transferred bytes :  {endPoint} ");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacketId : {id}, Size {size}");


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