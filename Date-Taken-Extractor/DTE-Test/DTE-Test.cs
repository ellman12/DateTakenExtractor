using System;
using Date_Taken_Extractor;
using Xunit;
using Xunit.Abstractions;

namespace DTE_Test;

public class DTE_Test
{
	private readonly ITestOutputHelper _testOutputHelper;

	public DTE_Test(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

	[Theory]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/29e5607b885099d7ba2d3fbb88e3328a9ba92ecc.mp4")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/DSC_6663.MOV")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/Snapchat-147976829.mp4")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/VID_20201012_142818.mp4")]
	[InlineData("C:/Users/Elliott/Videos/test/ms-CIJ1b2.gif")]
	[InlineData("C:/Users/Elliott/Videos/test/IMG_0083.JPG")]
	[InlineData("C:/Users/Elliott/Videos/test/IMG_20210320_175909.jpg")]
	[InlineData("C:/Users/Elliott/Videos/test/20210501_193046.mp4")]
	[InlineData("C:/Users/Elliott/Videos/test/20210502_144212.jpg")]
	[InlineData("C:/Users/Elliott/Videos/test/Capture 2020-12-26 21_05_58.png")]
	[InlineData("C:/Users/Elliott/Videos/test/Capture 2020-12-26 21_07_44.png")]
	[InlineData("C:/Users/Elliott/Videos/test/image.png")]
	[InlineData("C:/Users/Elliott/Videos/test/image0.png")]
	[InlineData("C:/Users/Elliott/Videos/test/hell.jpg")]
	[InlineData("C:/Users/Elliott/Videos/test/unknown.png")]
	[InlineData("C:/Users/Elliott/Videos/test/VID_20210320_173313..mp4")]
	[InlineData("C:/Users/Elliott/Videos/test/s-that-a-higher-res-image-of-the-perhaps-cow-49863050.png")]
	[InlineData("C:/Users/Elliott/Videos/test/Screenshot 2020-11-24 101446.png")]
	[InlineData("C:/Users/Elliott/Videos/test/Pie Flavor Profile Picture GIF.gif")]
	[InlineData("C:/Users/Elliott/Videos/test/3dgb9j.png")]
	[InlineData("C:/Users/Elliott/Videos/test/3wxq2e.png")]
	[InlineData("C:/Users/Elliott/Videos/test/85v6wd8p50x61.jpg")]
	[InlineData("C:/Users/Elliott/Videos/test/200.gif")]
	[InlineData("C:/Users/Elliott/Videos/test/03i2uu7fvao41.jpg")]
	[InlineData("C:/Users/Elliott/Videos/test/888et9kgogn_(35).gif")]
	[InlineData("C:/Users/Elliott/Videos/test/2020-10-06_13.53.33.png")]
	[InlineData("C:/Users/Elliott/Videos/test/105600_20201122160252_1.png")]
	[InlineData("C:/Users/Elliott/Videos/test/20210426_094833.jpg")]
	[InlineData("C:/Users/Elliott/Videos/test/20210426_095923.jpg")]
	[InlineData("C:/Users/Elliott/Videos/test/20210426_101404.jpg")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/IMG_0841.MOV")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/IMG_1027.MOV")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/IMG_1341.MOV")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/TWUC6365.MOV")]
	[InlineData("C:/Users/Elliott/Videos/test/angleordevil.png")]
	public void GetDateTakenFromMetadataTest(string fullPath)
	{
		DateTime? result = DateTakenExtractor.GetDateTakenFromMetadata(fullPath, out DateTakenExtractor.DateTakenSrc dateTakenSrc);
		_testOutputHelper.WriteLine(result == null ? "null" : result.ToString());
		_testOutputHelper.WriteLine(dateTakenSrc.ToString());
	}
}