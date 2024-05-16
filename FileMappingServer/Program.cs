// See https://aka.ms/new-console-template for more information
using ExampleTestClasses;
using FileMappingServer;

Console.WriteLine("I am Server!");
var testClass = new TestClass1();
using (var messagingService = new IPCServer<TestClass1>(testClass))
{
	messagingService.MessageReceived += (sender, args) => Console.WriteLine(args.message);
	messagingService.DoRun();
	while (true)
	{
		var name = nameof(testClass.GetTestValue);
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
		var arg = Console.ReadLine();
		await messagingService.Send(name, cancellationTokenSource.Token, arg); // this test class should be different.
	}
}
