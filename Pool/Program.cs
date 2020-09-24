using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PoolTask;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Pool pool = new Pool();
            pool.SetMaxThreads(1);
            int count = 0;
            for (int i = 0; i < 30; i++)
            {
                var task = new Task(() =>
                {
                    Thread.Sleep(10 * 1000);
                    Console.WriteLine("Count == {0}", count++);
                });
                Console.WriteLine(i);
                pool.QueueUserWorkItem(task);
            }
            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}
