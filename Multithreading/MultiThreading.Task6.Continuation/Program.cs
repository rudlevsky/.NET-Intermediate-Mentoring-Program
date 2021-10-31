/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            var parentA = Task.Run(() =>
            {
                Console.WriteLine("Parent task A");
            });

            var taskA = parentA.ContinueWith(t =>
            {
                Console.WriteLine("Continuation task A");
            });

            taskA.Wait();

            var parentB = Task.Run(() =>
            {
                Console.WriteLine("Parent task B");
                throw new Exception();
            });

            var taskB = parentB.ContinueWith(task =>
            {
                Console.WriteLine("Continuation task B");

                if (task.Exception != null)
                {
                    Console.WriteLine(task.Exception.Message);
                }
            }, TaskContinuationOptions.NotOnRanToCompletion);

            taskB.Wait();

            var parentC = Task.Run(() =>
            {
                Console.WriteLine("Parent task C");
                Console.WriteLine("Parent thread id: " + Thread.CurrentThread.ManagedThreadId);
                throw new Exception();
            });

            var taskC = parentC.ContinueWith(task =>
            {
                Console.WriteLine("Continuation task C");
                Console.WriteLine("Parent task exception: " + task.Exception.Message);
                Console.WriteLine("Continuation task thread id: " + Thread.CurrentThread.ManagedThreadId);
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);

            taskC.Wait();

            var tcs = new TaskCompletionSource<object>();
            var parentD = tcs.Task;

            var taskD = parentD.ContinueWith(task =>
            {
                Console.WriteLine("Continuation task D");
                Console.WriteLine("Is thread pool thread: " + Thread.CurrentThread.IsThreadPoolThread);
            }, TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning);

            tcs.SetCanceled();

            taskD.Wait();

            Console.ReadLine();
        }
    }
}
