﻿using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileMappingInterface;
namespace FileMappingClient
{
	internal class IPCClient<T> : IDisposable where T : class
	{
		public event EventHandler<MessageArgs>? MessageReceived;
		private PipeStream _readOnlyPipe;
		private ReadOnlyPipeExtension? _readOnlyPipeExtension;
		private NamedPipeClientStream? _writeOnlyPipe;
		private WriteOnlyPipeExtension? _writeOnlyPipeExtension;
		private InteropPipe<T> _interop;
		private T _interfaceMethods;

		public IPCClient(T interfaceMethods)
		{
			this._interfaceMethods = interfaceMethods;
			var readOnlyPipe = new NamedPipeClientStream(FileMappingConsts.SERVER_LOCATION, FileMappingConsts.PIPE_ONE_ClIENT_READ_SERVER_WRITE, PipeDirection.InOut);
			Console.WriteLine("Connecting...");
			readOnlyPipe.Connect(); // Connect to the pipe or throw an exception if unable to connect.
			readOnlyPipe.ReadMode = PipeTransmissionMode.Message;
			this._readOnlyPipe = readOnlyPipe;
			Console.WriteLine("Connected.");

			var writeOnlyPipe = new NamedPipeClientStream(FileMappingConsts.SERVER_LOCATION, FileMappingConsts.PIPE_TWO_CLIENT_WRITE_SERVER_READ, PipeDirection.InOut);
			Console.WriteLine("Connecting...");
			writeOnlyPipe.Connect(); // Connect to the pipe or throw an exception if unable to connect.
			this._writeOnlyPipe = writeOnlyPipe;
			Console.WriteLine("Connected.");
		}
		public void DoRun()
		{
			_readOnlyPipeExtension = new ReadOnlyPipeExtension(this._readOnlyPipe);
			_writeOnlyPipeExtension = new WriteOnlyPipeExtension(this._writeOnlyPipe);
			_interop = new InteropPipe<T>(_interfaceMethods, _writeOnlyPipeExtension, _readOnlyPipeExtension);
			_readOnlyPipeExtension.MessageReceived += MessageReceived;
		}
		public async Task<bool> WriteAsync(string message)
		{
			return await _interop.WriteMessageAsync(message);
		}
		public void Dispose()
		{
			_readOnlyPipeExtension.MessageReceived -= MessageReceived;
			_writeOnlyPipe?.Dispose();
			_readOnlyPipe?.Dispose();
		}
		public async Task<string> Send(string methodName, CancellationToken token, params string[] args)
		{
			return await _interop.Send(methodName, token, args);
		}
	}
}
