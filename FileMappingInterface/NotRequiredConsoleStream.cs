using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMappingInterface
{
	public class NotRequiredConsoleStream : Stream
	{
		public override bool CanRead => false;

		public override bool CanSeek => false;

		public override bool CanWrite => true;

		public override long Length => 0;

		public override long Position { get; set; }

		public override void Flush()
		{

		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return 0;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return 0;
		}
		public event EventHandler<MessageArgs> MessageReceived;
		public override void SetLength(long value)
		{

		}
		Queue<byte> information = new Queue<byte>();
		Queue<string> messages = new Queue<string>();
		int currentMessageLen = 0;
		private UnicodeEncoding streamEncoding = new UnicodeEncoding();

		public override void Write(byte[] buffer, int offset, int count)
		{
			for (int i = offset; i < count; i++)
			{
				information.Enqueue(buffer[i]);
			}
			if (information.Count > 4 && currentMessageLen == 0)
			{
				var lenBytes = new byte[4];
				for (int i = 0; i < 4; i++)
				{
					lenBytes[i] = information.Dequeue();
				}
				currentMessageLen = (int)BitConverter.ToUInt32(lenBytes, 0);
			}
			if (information.Count() >= currentMessageLen)
			{
				var messageBytes = new byte[currentMessageLen];
				for (int i = 0; i < currentMessageLen; i++)
				{
					messageBytes[i] = information.Dequeue();
				}
				// process message
				currentMessageLen = 0;
				var message = streamEncoding.GetString(messageBytes);
				messages.Enqueue(message);
			}
			if (messages.Count > 0)
			{
				MessageReceived?.BeginInvoke(this, new MessageArgs(messages.Dequeue()), null, null);
			}
		}
	}
}
