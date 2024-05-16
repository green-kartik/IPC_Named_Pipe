using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileMappingInterface
{
	public class InteropPipe<T> where T : class
	{
		private WriteOnlyPipeExtension _writeOnlyPipeExtension;
		private ReadOnlyPipeExtension _readOnlyPipeExtension;
		private Dictionary<string, string> _methodIdsAndResults = new Dictionary<string, string>();
		// we need guard this dictionary with a lock
		public static class ReturnTypes
		{
			public static string _NotReturned = "null";
			public static string _VoidReturn = "void";
		}
		private T _interfaceMethods;

		public InteropPipe(T interfaceMethods, WriteOnlyPipeExtension writeOnlyPipeExtension, ReadOnlyPipeExtension readOnlyPipeExtension) 
		{
			_interfaceMethods = interfaceMethods;
			_writeOnlyPipeExtension = writeOnlyPipeExtension;
			_readOnlyPipeExtension = readOnlyPipeExtension;
			readOnlyPipeExtension.MessageReceived += _readOnlyPipeExtension_MessageReceived;
		}
		private string Sender = "S";
		private string Receiver = "R";
		string breakchar = ((char)(' ' - 30)).ToString();
		public async Task<bool> WriteMessageAsync(string message)
		{
			return await _writeOnlyPipeExtension.WriteAsync("U" + breakchar + message);
		}

		//r , Id , returnType, value

		public void _readOnlyPipeExtension_MessageReceived(object? sender, MessageArgs e)
		{
			var messsage = e.message;
			var segments = messsage.Split(breakchar);
			if (segments[0] == Receiver)
			{
				if (_methodIdsAndResults.ContainsKey(segments[1]))
				{
					_methodIdsAndResults[segments[1]] = segments[2] != ReturnTypes._VoidReturn ? segments[3] : "";
				}
			}
			else if (segments[0] == Sender)
			{
				MethodInfo[] methodInfos = _interfaceMethods.GetType()
						   .GetMethods(BindingFlags.Public | BindingFlags.Instance);
				foreach (MethodInfo methodInfo in methodInfos)
				{
					if (methodInfo.Name == segments[2])
					{
						var objs = new object[segments.Length - 3];
						for (int i = 3; i < segments.Length; i++)
						{
							objs[i - 3] = segments[i];
						}
						var rc = _interfaceMethods.GetType().GetMethod(segments[2]).Invoke(_interfaceMethods, objs);
						var returnValue = rc != null ? rc : "";

						_writeOnlyPipeExtension.WriteAsync
	(Receiver + breakchar + segments[1] + breakchar + segments[2] + breakchar + returnValue).GetAwaiter().GetResult();
						// we can also use fire and forget
					}
				}
			}
			// else unknown message
		}


		public async Task<string> Send(string methodName, CancellationToken token, params string[] args)
		{
			var ID = Guid.NewGuid().ToString();
			_methodIdsAndResults.Add(ID, ReturnTypes._NotReturned);
			try
			{
				await _writeOnlyPipeExtension.WriteAsync
					(Sender + breakchar + ID + breakchar + methodName + breakchar + string.Join(breakchar, args));
			}
			catch (OperationCanceledException ex)
			{
				_methodIdsAndResults.Remove(ID);
				return "";
			}
			// s , Id, method, args
			while (true)
			{
				if (_methodIdsAndResults[ID] == ReturnTypes._NotReturned)
				{
					if (token.IsCancellationRequested)
					{
						_methodIdsAndResults.Remove(ID);
						return "";
					}
					await Task.Delay(100);
				}
				else
				{
					break;
				}
			}
			var returnValue = _methodIdsAndResults[ID];
			_methodIdsAndResults.Remove(ID);
			return returnValue;
		}
	}
}
