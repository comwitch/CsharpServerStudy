// See https://aka.ms/new-console-template for more information
using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class Program
    {
        class GameSession : Session
        {
            public override void OnConnected(EndPoint endPoint)
            {
                Console.WriteLine($"Onconnected :  {endPoint} ");

                //보낸다
                for (int i = 0; i < 5; i++)
                {
                    byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World {i}");
                    Send(sendBuff);

                }

                //보낸다
                
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
                Console.WriteLine($" [From Server] {recvData}");

                return buffer.Count;
            }

            public override void OnSend(int numOfBytes)
            {
                Console.WriteLine($"Transferred bytes :  {numOfBytes} ");

            }
        }
        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint, ()=> { return new GameSession(); });
           

            while(true)
            {
                //휴대폰 설정



                try
                {
      
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(1000);
            }



            
        }
    }
}