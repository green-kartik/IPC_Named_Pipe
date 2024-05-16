# IPC_Named_Pipe

#### See Folder FileMappingServer for Server Example
#### See Folder FileMappingClient for Client Example

## Example Client Code 
``` 
Console.WriteLine("I am Client!");
var testClass = new ClientTestClass();
using (var messagingService = new IPCClient<ClientTestClass>
	(testClass, 
	FileMappingConsts.SERVER_LOCATION, 
	FileMappingConsts.PIPE_ONE_ClIENT_READ_SERVER_WRITE, 
	FileMappingConsts.PIPE_TWO_CLIENT_WRITE_SERVER_READ))
{
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
}
```

## Example Server Code

```
Console.WriteLine("I am Server!");
var testClass = new ServerTestClass();
using (var messagingService = new IPCServer<ServerTestClass>(testClass,
	FileMappingConsts.PIPE_ONE_ClIENT_READ_SERVER_WRITE,
	FileMappingConsts.PIPE_TWO_CLIENT_WRITE_SERVER_READ))
{
	//messagingService.MessageReceived += (sender, args) => Console.WriteLine(args.message);
	messagingService.DoRun();
	while (true)
	{
		var name = nameof(IClientInterface.GetClientAnswer);
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));
		var arg = Console.ReadLine();
		var ans = await messagingService.Send<string>(name, cancellationTokenSource.Token, arg); // this test class should be different.
		Console.WriteLine(ans);
	}
}
```

## How to use
- Create Interface for interop class in client and server
- implement class for those interfaces
### Client
- Client uses interface for server to send commands
- Pass the client class into the IPCClient
### Server
- Server uses interface for client to send commands
- Pass the server class into the IPCServer

#### See - ExampleTestClasses to example classes

## Future Plans
- Easier Interface
- Nuget Package