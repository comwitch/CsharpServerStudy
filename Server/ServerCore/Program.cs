// See https://aka.ms/new-console-template for more information
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
   
    class Program
    {
        /// <summary>
        /// 근성
        /// 양보
        /// 갑질
        /// </summary>
        /// 

        // [jobQueue]
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => { return $"My Name is {Thread.CurrentThread.ManagedThreadId}"; });
        //static string ThreadName2;


        static void WhoAmI()
        {
            //ThreadName.Value = $"My Name is {Thread.CurrentThread.ManagedThreadId}";

            bool repeat = ThreadName.IsValueCreated;
            if(repeat)
                Console.WriteLine(ThreadName.Value + "(repeat)");
            else
                Console.WriteLine(ThreadName.Value);


            
        }

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);


            ThreadName.Dispose();
        }
    }
}