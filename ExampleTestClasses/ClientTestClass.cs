namespace ExampleTestClasses
{
	public class ClientTestClass : IClientInterface
	{
		public string GetClientAnswer(string input)
		{
			return string.Format("Answer from CLIENT class. This is your input: {0}.", input);
		}
	}
}
