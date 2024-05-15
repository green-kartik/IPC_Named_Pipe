// See https://aka.ms/new-console-template for more information
using FileMappingClient;

Console.WriteLine("I am Client!");
using (var messagingService = new IPCClient())
{
	messagingService.MessageReceived += (sender, args) => Console.WriteLine(args.message);
	messagingService.DoRun();
	while (true)
	{
		await messagingService.WriteAsync(Console.ReadLine());
	}
}
