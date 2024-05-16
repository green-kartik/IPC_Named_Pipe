// See https://aka.ms/new-console-template for more information
using ExampleTestClasses;
using IPCClient;

Console.WriteLine("I am Client!");
var testClass = new ClientTestClass();
using (var messagingService = new IPCClient<ClientTestClass>
	(testClass, 
	FileMappingConsts.SERVER_LOCATION, 
	FileMappingConsts.PIPE_ONE_ClIENT_READ_SERVER_WRITE, 
	FileMappingConsts.PIPE_TWO_CLIENT_WRITE_SERVER_READ))
{
	//messagingService.MessageReceived += (sender, args) => Console.WriteLine(args.message);
	messagingService.DoRun();
	while (true)
	{
		var name = nameof(IServerInterface.GetServerAnswer);
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));
		var arg = Console.ReadLine();
		var ans = await messagingService.Send(name, cancellationTokenSource.Token, arg); // this test class should be different.
		Console.WriteLine(ans);
	}
}
