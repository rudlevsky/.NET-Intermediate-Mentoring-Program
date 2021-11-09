using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Receiver
{
	class Program
	{
		static string connectionString = "Endpoint=sb://mentoringqueue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=nc3aLzU6Wvm3YDfUwgSRkiECQgyS97n3YxjNj+seeNg=";
		static string queueName = "queue1";

		static IQueueClient client;
		static string outputPath = "C:\\Users\\Ilya_Rudlevsky\\Desktop\\Output";
		static string currentFile = string.Empty;
		static string currentFilePath = string.Empty;
		static int bytesCount = 0;

		static void Main(string[] args)
		{
			MainAsync().GetAwaiter().GetResult();
		}

		static async Task MainAsync()
		{
			var managementClient = new ManagementClient(connectionString);
			var allQueues = await managementClient.GetQueuesAsync();
			var foundQueue = allQueues.Where(q => q.Path == queueName.ToLower()).SingleOrDefault();

			if (foundQueue == null)
			{
				await managementClient.CreateQueueAsync(queueName);
			}

			client = new QueueClient(connectionString, queueName, ReceiveMode.ReceiveAndDelete);

			client.RegisterMessageHandler(ProcessMessagesAsync, new MessageHandlerOptions(ExceptionReceivedHandler)
			{
				MaxConcurrentCalls = 3,
				AutoComplete = false
			});

			Console.ReadLine();
			await client.CloseAsync();
		}

		static async Task ProcessMessagesAsync(Message message, CancellationToken token)
		{
			string fileName = message.Label;
			
			if (currentFile != fileName)
			{
				Console.WriteLine("Retrieving new file {0}", fileName);
				currentFilePath = Path.Combine(outputPath, fileName);
				currentFile = fileName;
			}

			var position = Convert.ToInt64(message.UserProperties["position"]);
			var size = Convert.ToInt32(message.UserProperties["size"]);
			var fileSize = Convert.ToInt64(message.UserProperties["fileSize"]);

			using (var fileStream = new FileStream(currentFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
			{
				fileStream.Position = position;
				fileStream.Write(message.Body, 0, size);
				fileStream.Flush();
			}

			bytesCount += size;
			Console.WriteLine("Current size: {0}", bytesCount);

			if (bytesCount == fileSize)
			{
				Console.WriteLine($"Received file {fileName} and saved it");
				bytesCount = 0;
			}

			await client.CompleteAsync(message.SystemProperties.LockToken);
		}

		private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
		{
			return Task.FromResult(arg.Exception);
		}
	}
}
