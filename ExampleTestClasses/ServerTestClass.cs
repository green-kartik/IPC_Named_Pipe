namespace ExampleTestClasses
{
	public class ServerTestClass : IServerInterface
	{
		public string GetServerAnswer(string input)
		{
			return string.Format("Answer from SERVER class. This is your input: {0}.", input);
		}
		public int Add(int a, int b)
		{
			return a + b;
		}

		public int GetRandom()
		{
			var random = new Random();
			return random.Next(0,101);
		}


		public int SubStract(int a, int b)
		{
			return a - b;
		}
	}
}
