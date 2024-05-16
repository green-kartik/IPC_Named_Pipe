using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleTestClasses
{
	public interface IServerInterface
	{
		public string GetServerAnswer(string input);
		public int Add(int a, int b);
		public int SubStract(int a, int b);
		public int GetRandom();
	}
}
