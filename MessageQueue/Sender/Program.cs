using Microsoft.Azure.ServiceBus;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sender
{
	class Program
	{
		static string connectionString = "Endpoint=sb://mentoringqueue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=nc3aLzU6Wvm3YDfUwgSRkiECQgyS97n3YxjNj+seeNg=";
		static string queueName = "queue1";

		static IQueueClient client;

		const int chunkSize = 256000;
		static string inputPath = "C:\\Users\\Ilya_Rudlevsky\\Desktop\\Input";

		static void Main(string[] args)
		{
			MainAsync().GetAwaiter().GetResult();
		}

		static async Task MainAsync()
		{
			var path = Path.Combine(inputPath);
			var fileWatcher = new FileSystemWatcher(path);

			fileWatcher.Created += SendMessageAsync;
			fileWatcher.EnableRaisingEvents = true;

			client = new QueueClient(connectionString, queueName);

			Console.WriteLine("Press Enter to stop the service");

			var input = Console.ReadKey();
			if (input.Key == ConsoleKey.Enter)
			{
				await client.CloseAsync();
			}
		}

		static async void SendMessageAsync(object sender, FileSystemEventArgs e)
		{
			try
			{
				int sentCount = 0;
				Console.WriteLine($"Created: {e.FullPath}");

				using (FileStream fileStream = File.OpenRead(e.FullPath))
				{
					while (fileStream.Position != fileStream.Length)
					{
						var buffer = new byte[chunkSize];

						var message = new Message(buffer)
						{
							Label = e.Name
						};

						message.UserProperties.Add("position", fileStream.Position);

						var readBytes = fileStream.Read(buffer, 0, chunkSize);

						message.UserProperties.Add("size", readBytes);
						message.UserProperties.Add("fileSize", fileStream.Length);

						await client.SendAsync(message);

						sentCount += readBytes;
						Console.WriteLine("Sent: {0}", sentCount);
					}
				}

				sentCount = 0;
				Console.WriteLine($"File successfully sent");
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
