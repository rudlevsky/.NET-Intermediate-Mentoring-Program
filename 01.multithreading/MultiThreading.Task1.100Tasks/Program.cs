/*
 * 1.	Write a program, which creates an array of 100 Tasks, runs them and waits all of them are not finished.
 * Each Task should iterate from 1 to 1000 and print into the console the following string:
 * “Task #0 – {iteration number}”.
 */
using System;
using System.Threading.Tasks;

namespace MultiThreading.Task1._100Tasks
{
	class Program
    {
        const int TaskAmount = 100;
        const int MaxIterationsCount = 1000;

        static void Main(string[] args)
        {
            var tasks = new Task[TaskAmount];

            for (int i = 0; i < TaskAmount; i++)
			{
                tasks[i] = HundredTasks(i);
            }

            Task.WaitAll(tasks);
        }

		static Task HundredTasks(int number)
		{
			return Task.Run(() =>
			{
				for (int i = 0; i < MaxIterationsCount; i++)
				{
					Output(number, i);
				}
			});
		}

		static void Output(int taskNumber, int iterationNumber)
        {
            Console.WriteLine($"Task #{taskNumber} – {iterationNumber}");
        }
    }
}
