using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ImpromptuInterface;

namespace Chapter5.Recipe9
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
			string result = await GetDynamicAwaitableObject(true);
			Console.WriteLine(result);

			result = await GetDynamicAwaitableObject(false);
			Console.WriteLine(result);
		}

		static dynamic GetDynamicAwaitableObject(bool completeSynchronously)
		{
			dynamic result = new ExpandoObject();
			dynamic awaiter = new ExpandoObject();

			awaiter.Message = "Completed synchronously";
			awaiter.IsCompleted = completeSynchronously;
			awaiter.GetResult = (Func<string>)(() => awaiter.Message);

			awaiter.OnCompleted = (Action<Action>) ( callback => 
				ThreadPool.QueueUserWorkItem(state => {
					Thread.Sleep(TimeSpan.FromSeconds(1));
					awaiter.Message = GetInfo();
					if (callback != null) callback();
				})
			);

			IAwaiter<string> proxy = Impromptu.ActLike(awaiter);

			result.GetAwaiter = (Func<dynamic>) ( () => proxy );

			return result;
		}

		static string GetInfo()
		{
			return string.Format("Task is running on a thread id {0}. Is thread pool thread: {1}",
					Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
		}
	}

	public interface IAwaiter<T> : INotifyCompletion
	{
		bool IsCompleted { get; }

		T GetResult();
	}
}
