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

        static volatile int count = 0;
        static Lock _lock = new Lock();
        static int iterator = 100000;

        static void Main(string[] args)
        {

            Task t1 = new Task(delegate ()
            {
                for (int i = 0; i < iterator; i++)
                {
                    _lock.WriteLock();
                    _lock.WriteLock();
                    count--;
                    _lock.WriteUnlock();
                    _lock.WriteUnlock();
          
                }

            });


            Task t2 = new Task(delegate ()
            {
                for (int i = 0; i < iterator; i++)
                {
                    _lock.WriteLock();
                    count++;
                    _lock.WriteUnlock();
                }

            });


            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(count);
        }
    }
}