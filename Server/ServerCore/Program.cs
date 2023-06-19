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

        //상호배제
        static object _lock = new object();
        static SpinLock _lock2 = new SpinLock();
        static Mutex _lock3 = new Mutex();

        //[] [] []
        //reader writer lock slim

        class Reward { }

        static ReaderWriterLockSlim _lock4 = new ReaderWriterLockSlim();

        static Reward GetRewardById(int id)
        {
            _lock4.EnterReadLock();

            _lock4.ExitReadLock();
            
            
            
            return null;
        }

        static void AddReward(Reward reward)
        {
            _lock4.EnterWriteLock();

            _lock4.ExitWriteLock();
            lock (_lock)
            {

            }

        }



        static void Main(string[] args)
        {
            lock(_lock)
            {

            }
        }
    }
}