using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Chapter5.Recipe8
{
	class Program
	{
		static void Main(string[] args)
		{
			Task t = AsynchronousProcessing();
			t.Wait();
		}

		async static Task AsynchronousProcessing()
		{
			var sync = new CustomAwaitable(true);
			string result = await sync;
			Console.WriteLine(result);

			var async = new CustomAwaitable(false);
			result = await async;

			Console.WriteLine(result);
		}

		class CustomAwaitable
		{
			public CustomAwaitable(bool completeSynchronously)
			{
				_completeSynchronously = completeSynchronously;
			}

			public CustomAwaiter GetAwaiter()
			{
				return new CustomAwaiter(_completeSynchronously);
			}

			private readonly bool _completeSynchronously;
		}

		class CustomAwaiter : INotifyCompletion
		{
			private string _result = "Completed synchronously";
			private readonly bool _completeSynchronously;

			public bool IsCompleted { get { return _completeSynchronously; } }

			public CustomAwaiter(bool completeSynchronously)
			{
				_completeSynchronously = completeSynchronously;
			}

			public string GetResult()
			{
				return _result;
			}

			public void OnCompleted(Action continuation)
			{
				ThreadPool.QueueUserWorkItem( state => {
					Thread.Sleep(TimeSpan.FromSeconds(1));
					_result = GetInfo();
					if (continuation != null) continuation();
				});
			}

			private string GetInfo()
			{
				return string.Format("Task is running on a thread id {0}. Is thread pool thread: {1}",
					Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
			}
		}
	}
}
