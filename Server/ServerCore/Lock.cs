using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCore
{
    // 재귀적 락을 허용할지(Yes)
    // 스핀락 정책 (5000번 -> yield)
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000;
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;
        
        /**
         * ReadCount : ReadLock을 획득했을때 여러쓰레드가 read를 잡을 수 있으니 카운팅해줌
         * WriteThreadId : WriteLock은 한 스레드만 획득이 가능하므로 그 ID를 저장해 놓는다
         *  재귀적으로 사용했을때 write lock read lock을 했다면 read unlock write Unlock순서대로 해줘야 한다.
         */
        // [Unused(1)][WriteThreadId(15bit)][ReadCount(16bit)]
        int _flag = EMPTY_FLAG;
        int _writeCount = 0; //애초에 상호 배타적이라 따로 변수로 빼줘도 됨

        public void WriteLock()
        {
            // 동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                _writeCount++;
                return;
            }


            /**
             *  아무도 WriteLock or ReadLock을 획득하고 있지 않을 떄, 경합해서 소유권을 얻는다
             */
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            while(true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    //시도를 해서 성공하면 return

                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                }

                Thread.Yield();
            }

            
        }

        public void WriteUnlock()
        {
            int lockCount = --_writeCount;
            if (lockCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            while (true)
            {
                for(int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK);  
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected) // A(0 -> 1) B(0 -> 1)
                        return;
                }

                Thread.Yield();
            }

        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }


    }
}
