using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SequentialTaskScheduler
{
    /// <summary>
    /// Executes tasks sequentially in order they were added by AddForExecution method
    /// </summary>
    public class TaskScheduler
    {
        private readonly ConcurrentQueue<Action> tasks = new ConcurrentQueue<Action>();
        private readonly List<Exception> innerExceptions = new List<Exception>();
        private readonly object syncRoot = new object();    // Synchronization instance to create the worker
        private volatile Task worker;  // Microsoft recommend to use volatile to ensure that assignment to the worker completes before the worker can be accessed

        /// <summary>
        /// Returns set of exceptions, that occurred during the execution of tasks
        /// </summary>
        public AggregateException Exception => innerExceptions.Count > 0 ? new AggregateException(innerExceptions) : null;

        /// <summary>
        /// Return current status of the worker. True, if worker is executing task, otherwise - false
        /// </summary>
        public bool IsBusy => worker != null && !worker.IsCompleted;

        /// <summary>
        /// Adds an action to the execution queue
        /// </summary>
        /// <param name="action">The instance of an action. You mustn't pass null as an action</param>
        public void AddForExecution(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            tasks.Enqueue(action);
            StartWorker();
        }

        /// <summary>
        /// Waits for the TaskScheduler to complete execution
        /// </summary>
        public void Wait()
        {
            worker?.Wait();
        }

        /// <summary>
        /// Starts the worker task
        /// </summary>
        private void StartWorker()
        {
            var needNewWorker = !tasks.IsEmpty && !IsBusy;
            // Double check pattern
            if (needNewWorker)  
            {
                lock (syncRoot)
                {
                    if (needNewWorker)
                    {
                        worker = Task.Factory.StartNew(WorkerMethod);
                    }
                }
            }
        }

        /// <summary>
        /// Gets tasks from the queue and executes them
        /// </summary>
        private void WorkerMethod()
        {
            while (!tasks.IsEmpty)
            {
                if (tasks.TryDequeue(out var result))
                {
                    try
                    {
                        result.Invoke();
                    }
                    catch (Exception ex)
                    {
                        innerExceptions.Add(ex);
                    }
                }
            }
        }
    }
}
