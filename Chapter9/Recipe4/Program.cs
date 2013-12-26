using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;

namespace Chapter9.Recipe4
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceHost host = null;

			try
			{
				host = new ServiceHost(typeof (HelloWorldService), new Uri(SERVICE_URL));
				var metadata = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
				if (null == metadata)
				{
					metadata = new ServiceMetadataBehavior();
				}

				metadata.HttpGetEnabled = true;
				metadata.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				host.Description.Behaviors.Add(metadata);

				host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(),
				                        "mex");
				var endpoint = host.AddServiceEndpoint(typeof (IHelloWorldService), new BasicHttpBinding(), SERVICE_URL);

				host.Faulted += (sender, e) => Console.WriteLine("Error!");

				host.Open();

				Console.WriteLine("Greeting service is running and listening on:");
				Console.WriteLine("{0} ({1})", endpoint.Address, endpoint.Binding.Name);

				var client = RunServiceClient();
				client.GetAwaiter().GetResult();

				Console.WriteLine("Press Enter to exit");
				Console.ReadLine();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in catch block: {0}", ex);
			}
			finally
			{
				if (null != host)
				{
					if (host.State == CommunicationState.Faulted)
					{
						host.Abort();
					}
					else
					{
						host.Close();
					}
				}
			}
		}

		const string SERVICE_URL = "http://localhost:1234/HelloWorld";

		static async Task RunServiceClient()
		{
			var endpoint = new EndpointAddress(SERVICE_URL);
			var channel = ChannelFactory<IHelloWorldServiceClient>.CreateChannel(new BasicHttpBinding(), endpoint);

			var greeting = await channel.GreetAsync("Eugene");
			Console.WriteLine(greeting);
		}

		[ServiceContract(Namespace = "Packt", Name = "HelloWorldServiceContract")]
		public interface IHelloWorldService
		{
			[OperationContract]
			string Greet(string name);
		}

		[ServiceContract(Namespace = "Packt", Name = "HelloWorldServiceContract")]
		public interface IHelloWorldServiceClient
		{
			[OperationContract]
			string Greet(string name);

			[OperationContract]
			Task<string> GreetAsync(string name);
		}

		public class HelloWorldService : IHelloWorldService
		{
			public string Greet(string name)
			{
				return string.Format("Greetings, {0}!", name);
			}
		}
	}
}