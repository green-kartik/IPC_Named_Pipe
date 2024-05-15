using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMappingInterface
{
	public class MessageArgs : EventArgs
	{
		public MessageArgs(string message)
			: base()
		{
			this.message = message;
		}
		public string message;
	}
}
