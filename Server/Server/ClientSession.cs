﻿using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    public enum PacketId
    {
        PlayerInfoReq = 1,
        Test = 2,

    }



    class PlayerInfoReq
    {
        public byte testByte;
        public long playerId;
        public string name;
        public struct Skill
        {
            public int id;
            public short level;
            public float duration;
            public struct Attribute
            {
                public int att;

                public void Read(ReadOnlySpan<byte> s, ref ushort count)
                {
                    this.att = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                    count += sizeof(int);
                }

                public bool Write(Span<byte> s, ref ushort count)
                {
                    bool success = true;

                    success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.att);
                    count += sizeof(int);
                    return success;
                }

            }

            public List<Attribute> attributes = new List<Attribute>();

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);
                this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);
                this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
                count += sizeof(float);
                this.attributes.Clear();
                ushort attributeLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
                count += sizeof(ushort);
                for (int i = 0; i < attributeLen; i++)
                {
                    Attribute attribute = new Attribute();
                    attribute.Read(s, ref count);
                    attributes.Add(attribute);
                }
            }

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.id);
                count += sizeof(int);

                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.level);
                count += sizeof(short);

                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.duration);
                count += sizeof(float);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)attributes.Count);
                count += sizeof(ushort);
                foreach (Attribute attribute in this.attributes)
                    success &= attribute.Write(s, ref count);
                return success;
            }

        }

        public List<Skill> skills = new List<Skill>();

        public void Read(ArraySegment<byte> segment)
        {
            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
            ushort count = 0;

            count += sizeof(ushort);
            count += sizeof(ushort);

            this.testByte = (byte)segment.Array[segment.Offset + count];
            count += sizeof(byte);
            this.playerId = BitConverter.ToUInt32(s.Slice(count, s.Length - count));
            count += sizeof(long);
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;
            this.skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            for (int i = 0; i < skillLen; i++)
            {
                Skill skill = new Skill();
                skill.Read(s, ref count);
                skills.Add(skill);
            }
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketId.PlayerInfoReq);
            count += sizeof(ushort);
            segment.Array[segment.Offset + count] = (byte)this.testByte;
            count += sizeof(byte);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long);
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);
            foreach (Skill skill in this.skills)
                success &= skill.Write(s, ref count);
            success &= BitConverter.TryWriteBytes(s, count);
            if (success == false)
                return null;
            return SendBufferHelper.Close(count);
        }
    }

    class Test
    {
        public int testInt;

        public void Read(ArraySegment<byte> segment)
        {
            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
            ushort count = 0;

            count += sizeof(ushort);
            count += sizeof(ushort);

            this.testInt = BitConverter.ToInt32(s.Slice(count, s.Length - count));
            count += sizeof(int);
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketId.Test);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.testInt);
            count += sizeof(int);
            success &= BitConverter.TryWriteBytes(s, count);
            if (success == false)
                return null;
            return SendBufferHelper.Close(count);
        }
    }



    class ClientSession : PacketSession
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
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            Console.WriteLine($"RecvPacketId : {id}, Size {size}");


            switch((PacketId)id)
            {
                case PacketId.PlayerInfoReq:
                    {
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"Player InfoReq: {p.playerId}, {p.name}");

                        foreach (PlayerInfoReq.Skill skill in p.skills)
                        {
                            Console.WriteLine($"Skill({skill.id}, {skill.level}, {skill.duration})");
                        }    

                        
                    }
                    break;
            }
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes :  {numOfBytes} ");

        }
    }

}
