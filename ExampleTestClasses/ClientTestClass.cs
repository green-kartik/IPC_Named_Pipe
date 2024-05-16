using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleTestClasses
{
	public class ClientTestClass
	{
		public string GetClientAnswer(string input)
		{
			return string.Format("Answer from CLIENT class. This is your input: {0}.", input);
		}
	}
}
