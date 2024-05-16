namespace ExampleTestClasses
{
	public class TestClass1 : ITestInterface1
	{
		public string GetTestValueName()
		{
			return nameof(GetTestValue);
		}
		public string GetTestValue(string input)
		{
			return string.Format("this is your input: {0}.", input);
		}
	}
}
