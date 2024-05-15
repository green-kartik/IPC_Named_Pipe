using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileMappingInterface
{
	public class ReadOnlyPipeExtension : IDisposable
	{
	    private PipeStream _pipe;
		private readonly int _timeoutReadActionInMilliseconds;

		public event EventHandler<MessageArgs> ?MessageReceived;

		private UnicodeEncoding _streamEncoding;
		private Thread _processIncomingMessageThread;
		private bool _disposed;

		public ReadOnlyPipeExtension(PipeStream readOnlyPipe, int timeoutReadActionInMilliseconds = 10000)
		{
			this._disposed = false;
			this._pipe = readOnlyPipe;
			this._timeoutReadActionInMilliseconds = timeoutReadActionInMilliseconds;
			this._streamEncoding = new UnicodeEncoding();
			_processIncomingMessageThread = new Thread(async () => await ProcessMessages());
			_processIncomingMessageThread.Start();
		}
		private async Task ProcessMessages()
		{
			while (!_disposed)
			{
				byte[] bytes = new byte[4];
				var mode = _pipe.ReadMode;
				var cancellationTokenSource = new CancellationTokenSource();
				var outLen = 0;
				cancellationTokenSource.CancelAfter(_timeoutReadActionInMilliseconds/2);
				try
				{
					await _pipe.FlushAsync();
					outLen = await _pipe.ReadAsync(bytes, 0, 4, cancellationTokenSource.Token);
				}
				catch (OperationCanceledException ex)
				{
					await Task.Delay(300); // time to recover if something went wrong
					continue;
				}
				if (outLen < 4)
				{
					while (!_pipe.IsMessageComplete) // clear the buffer for next time
					{
						var oneByte = new Byte[1];
						await _pipe.ReadAsync(oneByte, 0, 1);
						continue;
					}
				}
				int len = (int)BitConverter.ToUInt32(bytes, 0);
				var messageInBytes = new byte[len];
				cancellationTokenSource = new CancellationTokenSource();
				cancellationTokenSource.CancelAfter(_timeoutReadActionInMilliseconds);
				try
				{
					outLen = await _pipe.ReadAsync(messageInBytes, 0, len, cancellationTokenSource.Token);
				}
				catch (OperationCanceledException ex) {
					// this message is now garbage
				}

				while (!_pipe.IsMessageComplete) // clear the buffer for next time
				{
					var oneByte = new Byte[1];
					await _pipe.ReadAsync(oneByte, 0, 1);
				}
				var message = _streamEncoding.GetString(messageInBytes);
				MessageReceived?.Invoke(this, new MessageArgs(message));
			}
		}
		public void Dispose()
		{
			try
			{
				_disposed = true;
				if (_pipe != null)
				{
					_pipe.Dispose();
				}
			}
			catch (Exception ex)
			{
				// already disposed
			}
		}
	}
}
