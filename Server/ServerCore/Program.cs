// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{

    class Program
    {
        volatile static bool _stop = false;
       
        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작");

            while(_stop == false)
            {
                //someone want to be stop
            }

            Console.WriteLine("finish thread");
             

        }

        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("call stop");
            Console.WriteLine("wait finish");
            t.Wait();

            Console.WriteLine("finish success");
            
        }
    }
}