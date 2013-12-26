using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Chapter9.Recipe3
{
	class Program
	{
		static void Main(string[] args)
		{
			const string dataBaseName = "CustomDatabase";
			var t = ProcessAsynchronousIO(dataBaseName);
			t.GetAwaiter().GetResult();
			Console.WriteLine("Press Enter to exit");
			Console.ReadLine();
		}

		async static Task ProcessAsynchronousIO(string dbName)
		{
			try
			{
				const string connectionString = @"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True";
				string outputFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				string dbFileName = Path.Combine(outputFolder, string.Format(@".\{0}.mdf", dbName));
				string dbLogFileName = Path.Combine(outputFolder, string.Format(@".\{0}_log.ldf", dbName));
				string dbConnectionString = 
					string.Format(@"Data Source=(LocalDB)\v11.0;AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", dbName, dbFileName);

				using (var connection = new SqlConnection(connectionString))
				{
					await connection.OpenAsync();

					if (File.Exists(dbFileName))
					{
						Console.WriteLine("Detaching the database...");

						var detachCommand = new SqlCommand("sp_detach_db", connection);
						detachCommand.CommandType = CommandType.StoredProcedure;
						detachCommand.Parameters.AddWithValue("@dbname", dbName);

						await detachCommand.ExecuteNonQueryAsync();

						Console.WriteLine("The database was detached succesfully.");
						Console.WriteLine("Deleteing the database...");

						if(File.Exists(dbLogFileName)) File.Delete(dbLogFileName);
						File.Delete(dbFileName);

						Console.WriteLine("The database was deleted succesfully.");
					}

					Console.WriteLine("Creating the database...");
					string createCommand = String.Format("CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", dbName, dbFileName);
					var cmd = new SqlCommand(createCommand, connection);

					await cmd.ExecuteNonQueryAsync();
					Console.WriteLine("The database was created succesfully");
				}

				using (var connection = new SqlConnection(dbConnectionString))
				{
					await connection.OpenAsync();

					var cmd = new SqlCommand("SELECT newid()", connection);
					var result = await cmd.ExecuteScalarAsync();

					Console.WriteLine("New GUID from DataBase: {0}", result);

					cmd = new SqlCommand(@"CREATE TABLE [dbo].[CustomTable]( [ID] [int] IDENTITY(1,1) NOT NULL, [Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ID] PRIMARY KEY CLUSTERED ([ID] ASC) ON [PRIMARY]) ON [PRIMARY]", connection);
					await cmd.ExecuteNonQueryAsync();

					Console.WriteLine("Table was created succesfully.");

					cmd = new SqlCommand(@"INSERT INTO [dbo].[CustomTable] (Name) VALUES ('John');
INSERT INTO [dbo].[CustomTable] (Name) VALUES ('Peter');
INSERT INTO [dbo].[CustomTable] (Name) VALUES ('James');
INSERT INTO [dbo].[CustomTable] (Name) VALUES ('Eugene');", connection);
					await cmd.ExecuteNonQueryAsync();

					Console.WriteLine("Inserted data succesfully");
					Console.WriteLine("Reading data from table...");

					cmd = new SqlCommand(@"SELECT * FROM [dbo].[CustomTable]", connection);
					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							var id = reader.GetFieldValue<int>(0);
							var name = reader.GetFieldValue<string>(1);

							Console.WriteLine("Table row: Id {0}, Name {1}", id, name);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine("Error: {0}", ex.Message);
			}
		}
	}
}
