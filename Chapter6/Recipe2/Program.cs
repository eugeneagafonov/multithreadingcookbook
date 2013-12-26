using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Chapter6.Recipe2
{
	class Program
	{
		static void Main(string[] args)
		{
			Task t = RunProgram();
			t.Wait();
		}

		static async Task RunProgram()
		{
			var taskQueue = new ConcurrentQueue<CustomTask>();
			var cts = new CancellationTokenSource();

			var taskSource = Task.Run(() => TaskProducer(taskQueue));

			Task[] processors = new Task[4];
			for (int i = 1; i <= 4; i++)
			{
				string processorId = i.ToString();
				processors[i-1] = Task.Run(
					() => TaskProcessor(taskQueue, "Processor " + processorId, cts.Token));
			}

			await taskSource;
			cts.CancelAfter(TimeSpan.FromSeconds(2));

			await Task.WhenAll(processors);
		}

		static async Task TaskProducer(ConcurrentQueue<CustomTask> queue)
		{
			for (int i = 1; i <= 20; i++)
			{
				await Task.Delay(50);
				var workItem = new CustomTask {Id = i};
				queue.Enqueue(workItem);
				Console.WriteLine("Task {0} has been posted", workItem.Id);
			}
		}

		static async Task TaskProcessor(
			ConcurrentQueue<CustomTask> queue, string name, CancellationToken token)
		{
			CustomTask workItem;
			bool dequeueSuccesful = false;

			await GetRandomDelay();
			do
			{
				dequeueSuccesful = queue.TryDequeue(out workItem);
				if (dequeueSuccesful)
				{
					Console.WriteLine("Task {0} has been processed by {1}", workItem.Id, name);
				}

				await GetRandomDelay();
			}
			while (!token.IsCancellationRequested);
		}

		static Task GetRandomDelay()
		{
			int delay = new Random(DateTime.Now.Millisecond).Next(1, 500);
			return Task.Delay(delay);
		}

		class CustomTask
		{
			public int Id { get; set; }
		}
	}
}
