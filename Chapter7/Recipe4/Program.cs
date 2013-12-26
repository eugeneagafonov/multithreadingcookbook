using System;
using System.Collections.Generic;
using System.Linq;

namespace Chapter7.Recipe4
{
	class Program
	{
		static void Main(string[] args)
		{
			IEnumerable<int> numbers = Enumerable.Range(-5, 10);

			var query = from number in numbers
									select 100 / number;

			try
			{
				foreach(var n in query)
					Console.WriteLine(n);
			}
			catch (DivideByZeroException)
			{
				Console.WriteLine("Divided by zero!");
			}

			Console.WriteLine("---");
			Console.WriteLine("Sequential LINQ query processing");
			Console.WriteLine();

			var parallelQuery = from number in numbers.AsParallel()
													select 100 / number; ;

			try
			{
				parallelQuery.ForAll(Console.WriteLine);
			}
			catch (DivideByZeroException)
			{
				Console.WriteLine("Divided by zero - usual exception handler!");
			}
			catch (AggregateException e)
			{
				e.Flatten().Handle(ex =>
				{
					if (ex is DivideByZeroException)
					{
						Console.WriteLine("Divided by zero - aggregate exception handler!");
						return true;
					}
					
					return false;
				});
			}

			Console.WriteLine("---");
			Console.WriteLine("Parallel LINQ query processing and results merging");
		}
	}
}
