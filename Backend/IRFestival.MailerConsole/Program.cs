// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;
using IRFestival.Api.Domain;
using System.Text.Json;

Console.WriteLine("Hello, MailerConsole!");

var connectionString = "Endpoint=sb://irfestivalservicebusdd.servicebus.windows.net/;SharedAccessKeyName=listener;SharedAccessKey=WCuWgqQ8YxjI4QUud/YPJ1E1VXXQMpbtvZ5nJ5+Jdnk=;EntityPath=mails";
var queueName = "mails";

await using (var client = new ServiceBusClient(connectionString))
{
    var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;
    await processor.StartProcessingAsync();
    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();
    Console.WriteLine("\nStopping the receiver ...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
static async Task MessageHandler(ProcessMessageEventArgs args)
{
    var body = args.Message.Body.ToString();
    var message = JsonSerializer.Deserialize<Mail>(body);
    Console.WriteLine($"Mail to send : {message?.To}");
    await args.CompleteMessageAsync(args.Message);
}
static async Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    await Task.CompletedTask;
}
