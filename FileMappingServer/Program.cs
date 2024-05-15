// See https://aka.ms/new-console-template for more information
using FileMappingServer;

Console.WriteLine("I am Server!");
using (var messagingService = new IPCServer())
{
	messagingService.MessageReceived += (sender, args) => Console.WriteLine(args.message);
	messagingService.DoRun();
	while (true)
	{
		//var message = obj.GetMessage();
		//Console.WriteLine(message);
		await messagingService.WriteAsync(Console.ReadLine());
	}
//	Console.ReadKey();
}
