using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chapter4.Recipe4
{
	class Program
	{
		private static void Main(string[] args)
		{
			int threadId;
			AsynchronousTask d = Test;
			IncompatibleAsynchronousTask e = Test;

			Console.WriteLine("Option 1");
			Task<string> task = Task<string>.Factory.FromAsync(
				d.BeginInvoke("AsyncTaskThread", Callback, "a delegate asynchronous call"), d.EndInvoke);

			task.ContinueWith(t => Console.WriteLine("Callback is finished, now running a continuation! Result: {0}",
				t.Result));

			while (!task.IsCompleted)
			{
				Console.WriteLine(task.Status);
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
			}
			Console.WriteLine(task.Status);
			Thread.Sleep(TimeSpan.FromSeconds(1));

			Console.WriteLine("----------------------------------------------");
			Console.WriteLine();
			Console.WriteLine("Option 2");

			task = Task<string>.Factory.FromAsync(
				d.BeginInvoke, d.EndInvoke, "AsyncTaskThread", "a delegate asynchronous call");
			task.ContinueWith(t => Console.WriteLine("Task is completed, now running a continuation! Result: {0}",
				t.Result));
			while (!task.IsCompleted)
			{
				Console.WriteLine(task.Status);
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
			}
			Console.WriteLine(task.Status);
			Thread.Sleep(TimeSpan.FromSeconds(1));

			Console.WriteLine("----------------------------------------------");
			Console.WriteLine();
			Console.WriteLine("Option 3");

			IAsyncResult ar = e.BeginInvoke(out threadId, Callback, "a delegate asynchronous call");
			task = Task<string>.Factory.FromAsync(ar, _ => e.EndInvoke(out threadId, ar));
			task.ContinueWith(t => 
				Console.WriteLine("Task is completed, now running a continuation! Result: {0}, ThreadId: {1}",
					t.Result, threadId));

			while (!task.IsCompleted)
			{
				Console.WriteLine(task.Status);
				Thread.Sleep(TimeSpan.FromSeconds(0.5));
			}
			Console.WriteLine(task.Status);

			Thread.Sleep(TimeSpan.FromSeconds(1));
		}

		private delegate string AsynchronousTask(string threadName);
		private delegate string IncompatibleAsynchronousTask(out int threadId);

		private static void Callback(IAsyncResult ar)
		{
			Console.WriteLine("Starting a callback...");
			Console.WriteLine("State passed to a callbak: {0}", ar.AsyncState);
			Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
			Console.WriteLine("Thread pool worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
		}

		private static string Test(string threadName)
		{
			Console.WriteLine("Starting...");
			Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
			Thread.Sleep(TimeSpan.FromSeconds(2));
			Thread.CurrentThread.Name = threadName;
			return string.Format("Thread name: {0}", Thread.CurrentThread.Name);
		}

		private static string Test(out int threadId)
		{
			Console.WriteLine("Starting...");
			Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
			Thread.Sleep(TimeSpan.FromSeconds(2));
			threadId = Thread.CurrentThread.ManagedThreadId;
			return string.Format("Thread pool worker thread id was: {0}", threadId);
		}
	}
}