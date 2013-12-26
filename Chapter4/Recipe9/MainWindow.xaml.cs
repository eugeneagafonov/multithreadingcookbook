using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chapter4.Recipe9
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		void ButtonSync_Click(object sender, RoutedEventArgs e)
		{
			ContentTextBlock.Text = string.Empty;
			try
			{
				//string result = TaskMethod(TaskScheduler.FromCurrentSynchronizationContext()).Result;
				string result = TaskMethod().Result;
				ContentTextBlock.Text = result;
			}
			catch (Exception ex)
			{
				ContentTextBlock.Text = ex.InnerException.Message;
			}
		}

		void ButtonAsync_Click(object sender, RoutedEventArgs e)
		{
			ContentTextBlock.Text = string.Empty;
			Mouse.OverrideCursor = Cursors.Wait;
			Task<string> task = TaskMethod();
			task.ContinueWith(t => {
					ContentTextBlock.Text = t.Exception.InnerException.Message;
					Mouse.OverrideCursor = null;
				}, 
				CancellationToken.None,
				TaskContinuationOptions.OnlyOnFaulted,
				TaskScheduler.FromCurrentSynchronizationContext());
		}

		void ButtonAsyncOK_Click(object sender, RoutedEventArgs e)
		{
			ContentTextBlock.Text = string.Empty;
			Mouse.OverrideCursor = Cursors.Wait;
			Task<string> task = TaskMethod(TaskScheduler.FromCurrentSynchronizationContext());
			task.ContinueWith(t => Mouse.OverrideCursor = null,
				CancellationToken.None,
				TaskContinuationOptions.None,
				TaskScheduler.FromCurrentSynchronizationContext());
		}

		Task<string> TaskMethod()
		{
			return TaskMethod(TaskScheduler.Default);
		}

		Task<string> TaskMethod(TaskScheduler scheduler)
		{
			Task delay = Task.Delay(TimeSpan.FromSeconds(5));

			return delay.ContinueWith(t =>
			{
				string str = string.Format("Task is running on a thread id {0}. Is thread pool thread: {1}",
						Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
				ContentTextBlock.Text = str;
				return str;
			}, scheduler);
		}
	}
}
