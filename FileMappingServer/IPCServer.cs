using FileMappingInterface;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FileMappingServer
{
	internal class IPCServer : IDisposable
	{
		private PipeStream _writeOnlyPipe;
		private WriteOnlyPipeExtension _writeOnlyPipeExtension;
		private ReadOnlyPipeExtension _readOnlyPipeExtension;
		private NamedPipeServerStream _readOnlyPipe;

		public event EventHandler<MessageArgs> MessageReceived;
		public IPCServer()
		{
			var writeOnlyPipeMethodLocal = new NamedPipeServerStream(FileMappingConsts.PIPE_ONE_ClIENT_READ_SERVER_WRITE, PipeDirection.InOut, 1,PipeTransmissionMode.Message);// serverName, pipeName, PipeDirection.InOut))
			Console.WriteLine("Connecting...");
			writeOnlyPipeMethodLocal.WaitForConnection();
			this._writeOnlyPipe = writeOnlyPipeMethodLocal;
			Console.WriteLine("Connected.");

			// lets open read only server
			var readOnlyPipeMethodLocal = new NamedPipeServerStream(FileMappingConsts.PIPE_TWO_CLIENT_WRITE_SERVER_READ, PipeDirection.InOut, 1, PipeTransmissionMode.Message);// serverName, pipeName, PipeDirection.InOut))
			Console.WriteLine("Connecting...");
			readOnlyPipeMethodLocal.ReadMode = PipeTransmissionMode.Message;
			readOnlyPipeMethodLocal.WaitForConnection();
			this._readOnlyPipe = readOnlyPipeMethodLocal;
			Console.WriteLine("Connected.");
		}
		public void DoRun()
		{
			_writeOnlyPipeExtension = new WriteOnlyPipeExtension(_writeOnlyPipe);
			_readOnlyPipeExtension = new ReadOnlyPipeExtension(_readOnlyPipe);
			_readOnlyPipeExtension.MessageReceived += MessageReceived;

		}

		public async Task<bool> WriteAsync(string message)
		{
			return await _writeOnlyPipeExtension.WriteAsync(message);
		}

		public void Dispose()
		{
			_writeOnlyPipe?.Dispose();
			_writeOnlyPipeExtension.Dispose();
			_readOnlyPipe.Dispose();
			_readOnlyPipeExtension.Dispose();
		}
	}
}
