﻿// See https://aka.ms/new-console-template for more information
using ExampleTestClasses;
using IPCClient;
using System.Threading;
using System.Xml.Linq;

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

	var name = nameof(IServerInterface.GetRandom);
	var cancellationTokenSource = new CancellationTokenSource();
	cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));
	var ans = await messagingService.Send<int>(name, cancellationTokenSource.Token); // this test class should be different.
	Console.WriteLine(string.Format("Server returned random number: {0}.", ans));

	name = nameof(IServerInterface.Add);
	cancellationTokenSource = new CancellationTokenSource();
	cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));
	ans = await messagingService.Send<int>(name, cancellationTokenSource.Token, 5 , 5); // this test class should be different.
	Console.WriteLine(string.Format("Server added {0} and {1} : {2}.", 5 , 5 ,ans));

	name = nameof(IServerInterface.SubStract);
	cancellationTokenSource = new CancellationTokenSource();
	cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));
	ans = await messagingService.Send<int>(name, cancellationTokenSource.Token, 5,5); // this test class should be different.
	Console.WriteLine(string.Format("Server subtracted {0} and {1} : {2}.", 5, 5, ans));

	while (true)
	{
		name = nameof(IServerInterface.GetServerAnswer);
		cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));
		var arg = Console.ReadLine();
		var stringAns = await messagingService.Send<string>(name, cancellationTokenSource.Token, arg); // this test class should be different.
		Console.WriteLine(stringAns);
	}
}
