using Xunit;
using Xunit.Abstractions;
using D = DateTakenExtractor.DateTakenExtractor;

namespace DTE_Test;

public class DTETest
{
	private readonly ITestOutputHelper _testOutputHelper;

	public DTETest(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

	[Theory]
	[InlineData("C:/Users/Elliott/Videos/original/413150_20220223191121_1.png")]
	[InlineData("C:/Users/Elliott/Videos/sorted/2022/2 February/23/413150_20220223190321_1 - Copy - Copy - Copy.png")]
	public void ExifToolPngReadTest(string fullPath)
	{
		_testOutputHelper.WriteLine(D.GetDateTakenAuto(fullPath, out _).ToString());
	}
}