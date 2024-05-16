using FileMappingInterface;
using System.IO.Pipes;

namespace IPCServer
{
	public class IPCServer<T> : IDisposable where T : class
	{
		private PipeStream _writeOnlyPipe;
		private WriteOnlyPipeExtension _writeOnlyPipeExtension;
		private ReadOnlyPipeExtension _readOnlyPipeExtension;
		private PipeStream _readOnlyPipe;

		public event EventHandler<MessageArgs> MessageReceived;
		private T _interfaceMethods;

		public IPCServer(T interfaceMethods, string serverWriteName, string serverReadName)
		{

			this._interfaceMethods = interfaceMethods;
			var writeOnlyPipeMethodLocal = new NamedPipeServerStream(serverWriteName, PipeDirection.InOut, 1, PipeTransmissionMode.Message);// serverName, pipeName, PipeDirection.InOut))
																																			//Console.WriteLine("Connecting...");
			writeOnlyPipeMethodLocal.WaitForConnection();
			this._writeOnlyPipe = writeOnlyPipeMethodLocal;
			//Console.WriteLine("Connected.");

			// lets open read only server
			var readOnlyPipeMethodLocal = new NamedPipeServerStream(serverReadName, PipeDirection.InOut, 1, PipeTransmissionMode.Message);// serverName, pipeName, PipeDirection.InOut))
																																		  //Console.WriteLine("Connecting...");
			readOnlyPipeMethodLocal.ReadMode = PipeTransmissionMode.Message;
			readOnlyPipeMethodLocal.WaitForConnection();
			this._readOnlyPipe = readOnlyPipeMethodLocal;
			//Console.WriteLine("Connected.");
		}
		public void DoRun()
		{
			_writeOnlyPipeExtension = new WriteOnlyPipeExtension(_writeOnlyPipe);
			_readOnlyPipeExtension = new ReadOnlyPipeExtension(_readOnlyPipe);
			_interop = new InteropPipe<T>(_interfaceMethods, _writeOnlyPipeExtension, _readOnlyPipeExtension);
			//_readOnlyPipeExtension.MessageReceived += MessageReceived;
		}

		public async Task<bool> WriteMessageAsync(string message)
		{
			return await _interop.WriteMessageAsync(message);
		}

		public void Dispose()
		{
			//_readOnlyPipeExtension.MessageReceived -= MessageReceived;
			_writeOnlyPipe?.Dispose();
			_readOnlyPipe.Dispose();
		}
		private InteropPipe<T> _interop;

		public async Task<string> Send(string methodName, CancellationToken token, params string[] args)
		{
			return await _interop.Send(methodName, token, args);
		}
	}
}
