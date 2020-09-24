using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using System.Diagnostics;

namespace PoolTask
{
    public class Pool
    {
        BlockingCollection<Task> block;
        BlockingCollection<Task> worker_collection;
        event Action get;
        private void Worker()
        {
            lock (block_obj)
            {
                Clear();
                AddNewTask();
            }
        }
        private void AddNewTask()
        {

            Log("AddNewTask");
            if (worker_collection.Count > maxWorkerThreads) return;
            Log("AddNewTaskGood");
            if (block.Count != 0)
            {

                var task =  block.ElementAtOrDefault(0);
                if (task == null) return;
                if ((int)task.Status > 2) 
                {
                    Log("return AddNewTask");
                    return;
                }
                task.Start();
                var becap = task;
                block.TryTake(out becap);
                task.ContinueWith(x => get.Invoke(), TaskContinuationOptions.ExecuteSynchronously);
                worker_collection.Add(task);
            }
        }
        private void Log(string msg)
        {
        #if DEBUG
            Console.WriteLine(msg);
        #endif
        }
        private void Clear()
        {
            Log("Clear Tasks");
            var remove = worker_collection
                .Where(x => (int)x.Status >= 4);

            foreach (var item in remove)
            {
                var x = item;
                worker_collection.TryTake(out x);
            }
        }
        public void QueueUserWorkItem(Task task)
        {
            Log("QueueUserWorkItem");
            block.Add(task);
            this.get.Invoke();
        }
        int maxWorkerThreads;
        /// <summary>
        /// Максимальна кількість потоків
        /// </summary>
        /// <param name="workerThreads"></param>
        /// <returns></returns>
        public void SetMaxThreads(int maxWorkerThreads)
        {
            this.maxWorkerThreads = maxWorkerThreads;
        }
        public int GetMaxThreads()
        {
            return this.maxWorkerThreads;
        }
        object block_obj;
        public Pool()
        {
            this.block = new BlockingCollection<Task>();
            this.worker_collection = new BlockingCollection<Task>();
            this.get += this.Worker;
            this.block_obj = new object();
        }
    }
}
