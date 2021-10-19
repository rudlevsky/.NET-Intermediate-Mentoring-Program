/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static Semaphore sem = new Semaphore(1, 10);

        static void Main(string[] args)
        {
            // a)
            int value = 10;

            Thread thread = new Thread(DecrementThread);

            thread.Start(value);
            thread.Join();

            Console.WriteLine();

            // b)
            ThreadPool.QueueUserWorkItem(DecrementThreadPool, value);

            Console.ReadLine();
        }

        static void DecrementThread(object obj)
        {
            int value = (int)obj;

            if (value > 0)
            {
                Console.Write(value + " ");
                value--;
                var thread = new Thread(DecrementThread);

                thread.Start(value);
                thread.Join();
            }
        }

        static void DecrementThreadPool(object obj)
        {
            int value = (int)obj;

            if (value > 0)
            {
                sem.WaitOne();
                Console.Write(value + " ");
                value--;
                sem.Release();
                ThreadPool.QueueUserWorkItem(DecrementThreadPool, value);
            }
        }
    }
}
