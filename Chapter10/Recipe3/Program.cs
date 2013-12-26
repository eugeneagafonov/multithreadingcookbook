using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Chapter10.Recipe3
{
	class Program
	{
		static void Main(string[] args)
		{
			var t = ProcessAsynchronously();
			t.GetAwaiter().GetResult();
		}

		async static Task ProcessAsynchronously()
		{
			var cts = new CancellationTokenSource();

			Task.Run(() =>
			{
				if (Console.ReadKey().KeyChar == 'c')
					cts.Cancel();
			});

			var inputBlock = new BufferBlock<int>(
				new DataflowBlockOptions { BoundedCapacity = 5, CancellationToken = cts.Token });

			var filter1Block = new TransformBlock<int, decimal>(
				n =>
				{
					decimal result = Convert.ToDecimal(n * 0.97);
					Console.WriteLine("Filter 1 sent {0} to the next stage on thread id {1}", result,
						Thread.CurrentThread.ManagedThreadId);
					Thread.Sleep(TimeSpan.FromMilliseconds(100));
					return result;
				}
				, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4, CancellationToken = cts.Token });

			var filter2Block = new TransformBlock<decimal, string>(
				n =>
				{
					string result = string.Format("--{0}--", n);
					Console.WriteLine("Filter 2 sent {0} to the next stage on thread id {1}", result,
						Thread.CurrentThread.ManagedThreadId);
					Thread.Sleep(TimeSpan.FromMilliseconds(100));
					return result;
				}
				, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4, CancellationToken = cts.Token });

			var outputBlock = new ActionBlock<string>(
				s =>
				{
					Console.WriteLine("The final result is {0} on thread id {1}",
						s, Thread.CurrentThread.ManagedThreadId);
				}
				, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4, CancellationToken = cts.Token });

			inputBlock.LinkTo(filter1Block, new DataflowLinkOptions { PropagateCompletion = true });
			filter1Block.LinkTo(filter2Block, new DataflowLinkOptions { PropagateCompletion = true });
			filter2Block.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });

			try
			{
				Parallel.For(0, 20, new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = cts.Token }
				, i =>
				{
					Console.WriteLine("added {0} to source data on thread id {1}", i, Thread.CurrentThread.ManagedThreadId);
					inputBlock.SendAsync(i).GetAwaiter().GetResult();
				});
				inputBlock.Complete();
				await outputBlock.Completion;
				Console.WriteLine("Press ENTER to exit.");
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("Operation has been canceled! Press ENTER to exit.");
			}

			Console.ReadLine();
		}
	}
}
