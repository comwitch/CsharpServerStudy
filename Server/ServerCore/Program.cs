// See https://aka.ms/new-console-template for more information
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Lock
    {
        //BOOL -> 커널
        AutoResetEvent _available = new AutoResetEvent(true);
        ManualResetEvent _available2 = new ManualResetEvent(false);
       

        public void Acquire()
        {
            _available2.WaitOne(); // 입장 시도
            _available2.Reset(); // <- Waitone에 다음이 있다. (auto에는 이럴 필요가 없다)
        }
        public void Release()
        {
            _available2.Set();
        }
    }

    class Program
    {
        static volatile int _num = 0;
        static Mutex _lock = new Mutex();
        
        static void Thread_1()
        {
            for (int i = 0; i<1000000; i++)
            {
                _lock.WaitOne();
                _num++;
                _lock.ReleaseMutex();

            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                _lock.WaitOne();
                _num--;
                _lock.ReleaseMutex();

            }
        }

        static void Main(string[] args)
        {

            Task t1 = new Task(Thread_1);  
            Task t2 = new Task(Thread_2);
            t2.Start();
            t1.Start();


            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }
}