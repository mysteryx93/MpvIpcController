using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace HanumanInstitute.MpvIpcController
{
    public static class ExtensionMethods
    {
        // https://medium.com/@alex.puiu/parallel-foreach-async-in-c-36756f8ebe62
        /// <summary>
        /// Loops through a list asynchronously in multiple threads.
        /// </summary>
        /// <typeparam name="T">The data type of the list to loop through.</typeparam>
        /// <param name="source">The list to loop through.</param>
        /// <param name="body">The work to execute for each item in the list.</param>
        /// <param name="maxDegreeOfParallelism">The maximum amount of concurrent threads to use for looping.</param>
        /// <param name="scheduler">The task schedule to use for creating threads.</param>
        /// <returns></returns>
        public static Task AsyncParallelForEach<T>(this IEnumerable<T> source, Func<T, Task> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded, TaskScheduler? scheduler = null)
        {
            source.CheckNotNull(nameof(source));

            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };
            if (scheduler != null)
            {
                options.TaskScheduler = scheduler;
            }

            var block = new ActionBlock<T>(body, options);

            foreach (var item in source)
            {
                block.Post(item);
            }

            block.Complete();
            return block.Completion;
        }
    }
}
