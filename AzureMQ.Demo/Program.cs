using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System;

namespace AzureMQ.Demo
{
    /// <summary>
    /// dotnet add package Microsoft.Azure.Storage.Queue
    /// https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions
    /// </summary>
    public class Program
    {
        public static string AzureConnectionString = "xxxx";

        public static string QueueName = "some-queue-demo";

        public static void Main(string[] args)
        {
            Console.WriteLine("Create azure mq client...");
            CloudStorageAccount account = CloudStorageAccount.Parse(AzureConnectionString);
            var client = account.CreateCloudQueueClient();

            Console.WriteLine("Get queue...");
            var queue = client.GetQueueReference(QueueName);

            Console.WriteLine("Create if not exists...");
            queue.CreateIfNotExists();

            Console.WriteLine("Add queue message...");
            Console.Write(" --> Message: ");
            var message = Console.ReadLine();
            var messageToSend = new CloudQueueMessage(message); // you can set execution time, retry policy, etc
            queue.AddMessage(messageToSend); // you can setup TTL, initial visilibity delay, etc

            Console.WriteLine("Get message...");
            queue.FetchAttributes();
            var messageCount = queue.ApproximateMessageCount;
            Console.WriteLine("Count: {0}", messageCount);

            Console.WriteLine("Get message...");
            var busyMessageInMinutes = TimeSpan.FromMinutes(5);
            var messageReceived = queue.GetMessage(busyMessageInMinutes);
            if (messageReceived != null)
            {
                Console.WriteLine("Message: {0}", messageReceived.AsString);

                Console.WriteLine("Return message to queue");
                queue.UpdateMessage(messageReceived, TimeSpan.FromSeconds(0), MessageUpdateFields.Visibility);

                Console.WriteLine("Delete message...");
                messageReceived = queue.GetMessage();
                queue.DeleteMessage(messageReceived);
            }
            else
            {
                Console.WriteLine("- without messages :(");
            }

            Console.ReadKey(); 
        }
    }
}
