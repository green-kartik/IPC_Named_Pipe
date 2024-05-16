using FileMappingInterface;
using System.IO.Pipes;

namespace IPCClient
{
	public class IPCClient<T> : IDisposable where T : class
	{
		public event EventHandler<MessageArgs>? MessageReceived;
		private PipeStream _readOnlyPipe;
		private ReadOnlyPipeExtension? _readOnlyPipeExtension;
		private NamedPipeClientStream? _writeOnlyPipe;
		private WriteOnlyPipeExtension? _writeOnlyPipeExtension;
		private InteropPipe<T> _interop;
		private T _interfaceMethods;

		public IPCClient(T interfaceMethods, string serverLocation, string clientReadName, string clientWriteName)
		{
			this._interfaceMethods = interfaceMethods;
			var readOnlyPipe = new NamedPipeClientStream(serverLocation, clientReadName, PipeDirection.InOut);
			//Console.WriteLine("Connecting...");
			readOnlyPipe.Connect(); // Connect to the pipe or throw an exception if unable to connect.
			readOnlyPipe.ReadMode = PipeTransmissionMode.Message;
			this._readOnlyPipe = readOnlyPipe;
			//Console.WriteLine("Connected.");

			var writeOnlyPipe = new NamedPipeClientStream(serverLocation, clientWriteName, PipeDirection.InOut);
			//Console.WriteLine("Connecting...");
			writeOnlyPipe.Connect(); // Connect to the pipe or throw an exception if unable to connect.
			this._writeOnlyPipe = writeOnlyPipe;
			//Console.WriteLine("Connected.");
		}
		public void DoRun()
		{
			_readOnlyPipeExtension = new ReadOnlyPipeExtension(this._readOnlyPipe);
			_writeOnlyPipeExtension = new WriteOnlyPipeExtension(this._writeOnlyPipe);
			_interop = new InteropPipe<T>(_interfaceMethods, _writeOnlyPipeExtension, _readOnlyPipeExtension);
			//_readOnlyPipeExtension.MessageReceived += MessageReceived;
		}
		public async Task<bool> WriteAsync(string message)
		{
			return await _interop.WriteMessageAsync(message);
		}
		public void Dispose()
		{
			//_readOnlyPipeExtension.MessageReceived -= MessageReceived;
			_writeOnlyPipe?.Dispose();
			_readOnlyPipe?.Dispose();
		}
		public async Task<string> Send(string methodName, CancellationToken token, params string[] args)
		{
			return await _interop.Send(methodName, token, args);
		}
	}
}
