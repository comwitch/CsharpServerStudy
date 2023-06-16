// See https://aka.ms/new-console-template for more information
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class SessionManager
    {
        static object _lock = new object();

        public static void TestSession()
        {
            lock (_lock)
            {

            }
        }

        public static void Test()
        {
            lock(_lock)
            {
                UserManager.TestUser();
            }
        }
    }

    class UserManager
    {
        static object _lock = new object();

        public static void Test()
        {

            lock (_lock)
            {
                SessionManager.TestSession();
            }
        }

        public static void TestUser()
        {
            lock( _lock)
            {
                
            }    
        } 
    }

    class Program
    {
        static int number = 0;
        static object _obj = new object();
        static object _obj2 = new object();

        //atomic 원자성 
        /**
         * 
         * 
         */
        static void Thread_1()
        {
            for (int i = 0; i < 10000; i++)
            {
                SessionManager.Test();
            }    
        }
        
        static void Thread_2()
        {
            for (int i = 0; i < 1000; i++)
            {
                UserManager.Test();
            }

        }

        // Deadlock lock을 안풀고 끝냈을때 
        static void Main(string[] args)
        {

            Task t1 = new Task(Thread_1);  
            Task t2 = new Task(Thread_2);
            t2.Start();
            t1.Start();


            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}