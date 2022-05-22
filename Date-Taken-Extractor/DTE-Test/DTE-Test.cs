using Xunit.Abstractions;

namespace DTE_Test;

public class DTE_Test
{
	private readonly ITestOutputHelper _testOutputHelper;

	public DTE_Test(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}
}