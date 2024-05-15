using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMappingInterface
{
	public class WriteOnlyPipeExtension : IDisposable
	{
		private PipeStream _pipe;
		private UnicodeEncoding _streamEncoding;

		public WriteOnlyPipeExtension(PipeStream writeOnlyPipe)
		{
			this._pipe = writeOnlyPipe;
			_streamEncoding = new UnicodeEncoding();
		}

		public void Dispose()
		{
			try
			{
				if (_pipe != null)
				{
					_pipe.Dispose();
				}
			}
			catch(Exception ex) {
				// already disposed
			}
		}

		public async Task<bool> WriteAsync(string message, int CancelAfterInMilliseconds = 3000)
		{
			if (string.IsNullOrEmpty(message))
			{
				message = "No message to send";
			}

			byte[] outBuffer = _streamEncoding.GetBytes(message);
			uint len = (uint)outBuffer.Length;
			byte[] bytes = new byte[4];

			bytes[3] = (byte)(len >> 24);
			bytes[2] = (byte)(len >> 16);
			bytes[1] = (byte)(len >> 8);
			bytes[0] = (byte)(len);
			var cancellationTokenSource = new CancellationTokenSource();
			cancellationTokenSource.CancelAfter(CancelAfterInMilliseconds);
			try
			{
				await _pipe.FlushAsync();
				await _pipe.WriteAsync(bytes, 0, bytes.Length, cancellationTokenSource.Token);
				await _pipe.WriteAsync(outBuffer, 0, outBuffer.Length, cancellationTokenSource.Token);
				return true;
			}
			catch (OperationCanceledException ex)
			{
				//await Task.Delay(CancelAter);
			}
			return false;
		}
	}
}
