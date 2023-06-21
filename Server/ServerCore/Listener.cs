using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;
        Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //문지기 교육
            _listenSocket.Bind(endPoint);
            //영업 시작
            //backlog : 최대 대기수 
            _listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            _onAcceptHandler += onAcceptHandler;
            //낙시대를 던진다.
            RegisterAccept(args);
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
            {
                OnAcceptCompleted(null, args);
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                //TODO
                _onAcceptHandler.Invoke(args.AcceptSocket);

            }
            else
                Console.WriteLine(args.SocketError.ToString());
            
            //낚시대를 다시 던진다.. 
            RegisterAccept(args);
        }

        public Socket Accept()
        {
            

            return _listenSocket.Accept();
        }


    }
}
