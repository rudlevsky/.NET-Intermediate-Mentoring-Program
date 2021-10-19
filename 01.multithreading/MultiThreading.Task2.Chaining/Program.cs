/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            const int arrayLength = 10;

            var task1 = Task.Run(() =>
            {
                var array = new int[arrayLength];

                for (int i = 0; i < arrayLength; i++)
                {
                    array[i] = rnd.Next(0, 100);
                }

                PrintArray(array);

                return array;
            });

            var task2 = task1.ContinueWith(task => {
                int[] array = task.Result;
                int randValue = rnd.Next();

                for (int i = 0; i < arrayLength; i++)
                {
                    array[i] *= rnd.Next(0, 100);
                }

                PrintArray(array);

                return array;
            });

            var task3 = task2.ContinueWith(task => {
                int[] array = task.Result;
                Array.Sort(array);

                PrintArray(array);

                return array;
            });

            var task4 = task3.ContinueWith(task => {
                int[] array = task.Result;

                var average = array.Average();

                Console.WriteLine($"Task #{Task.CurrentId}");
                Console.Write(average);
            });

            Task.WaitAll(task1, task2, task3, task4);
            Console.ReadLine();
        }

        static void PrintArray(int[] array)
		{
            Console.WriteLine($"Task #{Task.CurrentId}");

            foreach (int item in array)
            {
                Console.Write(item.ToString() + " ");
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
