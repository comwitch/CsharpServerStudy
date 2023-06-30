using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
    }

    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketId
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,   
    }

    class ServerSession : Session
    {
        static unsafe void ToBytes(byte[] array, int offset, ulong value)
        {
            fixed (byte* ptr = &array[offset])
                *(ulong*)ptr = value;
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Onconnectedccc :  {endPoint} ");
            PlayerInfoReq packet = new PlayerInfoReq() {  packetId = (ushort)PacketId.PlayerInfoReq,  playerId = 1001};

            //보낸다
            ArraySegment<byte> s = SendBufferHelper.Open(4096);

            
            ushort count = 0;
            bool success = true;

            //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), packet.size);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerId);
            count += 8;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

            //byte[] size = BitConverter.GetBytes(packet.size); //2byte
            //byte[] packetId = BitConverter.GetBytes(packet.packetId); //2byte
            //byte[] playerId = BitConverter.GetBytes(packet.playerId); // 8byte
            //Array.Copy(size, 0, s.Array, s.Offset + 0, 2);
            //count += 2;
            //Array.Copy(packetId, 0, s.Array, s.Offset + count, 2);
            //count += 2;
            //Array.Copy(playerId, 0, s.Array, s.Offset + count, 8);
            //count += 8;

            ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);

            if(success)
                Send(sendBuff);


            //보낸다

            //Thread.Sleep(1000);

            //Disconnect();
            //Disconnect();
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
}
