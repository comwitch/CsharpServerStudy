// See https://aka.ms/new-console-template for more information
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class SpinLock
    {
        volatile int _locked = 0;
        

        public void Acquire()
        {
            while(true)
            {
                //int original = Interlocked.Exchange(ref _locked, 1);
                //if (original == 0)
                //    break;

                // CAS Compare-And-Swap
                int expected = 0;
                int desired = 1;
                if(Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;

                //i want to rest
                //Thread.Sleep(1); // 1ms정도 무조건 쉬고 싶어요
                // Thread.Sleep(0); // 조건부 양보 -> 나보다 우선순위가 낮은 애들한테는 양보 불가 -> 우선순위가 나보다 같거나 높은 쓰레드가 없으면 다시 본인에게
                Thread.Yield(); // 관대한 양보 -> 관대하게 양보할테니, 지금 실행이 필요한 쓰레드가 있으면 실행하세요 -> 실행필요한거아니면 남은시간 소진.
            }
        }
        public void Release()
        {
            _locked = 0;
        }
    }

    class Program
    {
        static volatile int _num = 0;
        static SpinLock _lock = new SpinLock();
        
        static void Thread_1()
        {
            for (int i = 0; i<100000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();

            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Release();

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