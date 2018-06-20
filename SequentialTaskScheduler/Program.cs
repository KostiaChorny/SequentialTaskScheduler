using System;
using System.Threading;

namespace SequentialTaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var scheduler = new TaskScheduler();

            for (int i = 0; i < 10; i++)
            {
                var inner = i;
                scheduler.AddForExecution(() =>
                {
                    Console.WriteLine($"Exec started {inner}");
                    Thread.Sleep(1000);
                    Console.WriteLine($"Exec finished {inner}");
                });
            }

            scheduler.Wait();
            Console.WriteLine("End");
            Console.ReadKey();
        }
    }
}
