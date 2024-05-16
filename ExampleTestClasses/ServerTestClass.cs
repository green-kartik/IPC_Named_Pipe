namespace ExampleTestClasses
{
	public class ServerTestClass : IServerInterface
	{
		public string GetServerAnswer(string input)
		{
			return string.Format("Answer from SERVER class. This is your input: {0}.", input);
		}
	}
}
