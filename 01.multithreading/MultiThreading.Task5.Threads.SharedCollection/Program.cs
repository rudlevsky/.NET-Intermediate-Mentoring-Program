/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
		static AutoResetEvent addEvent = new AutoResetEvent(false);
		static AutoResetEvent printEvent = new AutoResetEvent(true);

		static List<int> collection = new List<int>();
		static int itemsCount = 10;

        static void Main(string[] args)
        {
            Task task1 = Task.Run(AddNumber);
            Task task2 = Task.Run(Print);

            Task.WaitAll(task1, task2);

            Console.ReadLine();
        }

		static void AddNumber()
		{
			for (int i = 0; i < itemsCount; i++)
			{
				printEvent.WaitOne();
				collection.Add(i);
				addEvent.Set();
			}
		}

		static void Print()
		{
			for (int i = 0; i < itemsCount; i++)
			{
				addEvent.WaitOne();
				Console.WriteLine();

				foreach (var item in collection)
				{
					Console.Write(item + " ");
				}
				printEvent.Set();
			}
		}
	}
}
