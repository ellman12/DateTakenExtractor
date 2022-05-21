using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Date_Taken_Extractor;
using Xunit;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.QuickTime;
using Xunit.Abstractions;
using Directory = MetadataExtractor.Directory;

namespace DTE_Test;

public class DTE_Test
{
	private readonly ITestOutputHelper _testOutputHelper;

	public DTE_Test(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

	[Fact]
	public void MDTest()
	{
		const string TEST_PATH = "C:/Users/Elliott/Videos/test/VID_20210429_160512.mp4";
		string[] files = System.IO.Directory.GetFiles("C:/Users/Elliott/Videos/", "*.mkv", SearchOption.AllDirectories);

		// foreach (string file in files)
		// {
			IEnumerable<MetadataExtractor.Directory> directories = QuickTimeMetadataReader.ReadMetadata(new FileStream(TEST_PATH, FileMode.Open));
			QuickTimeMovieHeaderDirectory yes = directories.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();

			yes.TryGetDateTime(QuickTimeMovieHeaderDirectory.TagCreated, out DateTime dateTaken);
			_testOutputHelper.WriteLine(dateTaken.ToString(CultureInfo.CurrentCulture));
			
			foreach (Directory directory in directories)
			foreach (Tag tag in directory.Tags)
				_testOutputHelper.WriteLine($"{directory.Name} - {tag.Name} = {tag.Description}");

			// _testOutputHelper.WriteLine(file);
			// if (subIfdDirectory == null)
			// {
			// 	_testOutputHelper.WriteLine("");
			// }
			// _testOutputHelper.WriteLine($"1: {subIfdDirectory.GetDescription(ExifDirectoryBase.TagDateTimeDigitized)}\t2: {subIfdDirectory.GetDescription(ExifDirectoryBase.TagDateTimeOriginal)}\t3: {subIfdDirectory.GetDescription(ExifDirectoryBase.TagDateTime)}\n");
		// }
	}

	[Theory]
	//Common filename timestamp formats I've encountered, with a few non-timestamps.
	[InlineData("IMG_20210320_175909.jpg")] //Android camera
	[InlineData("105600_20201226210642_1.png")] //Steam screenshots I think
	[InlineData("20201224140504_1.jpg")]
	[InlineData("Screenshot 2020-11-24 102029.png")] //Snip & Sketch
	[InlineData("Saved Clip 20201107143123.png")] //No idea
	[InlineData("Screenshot_2020-10-28_135904.png")] //No idea
	[InlineData("Screenshot_20210426-122329_Messages.jpg")] //Android screenshot
	[InlineData("2020-10-06_13.53.33.png")] //Minecraft I think
	[InlineData("Capture 2020-12-26 21_03_05.png")] //Terraria screenshot tool
	[InlineData("Snapchat-652999454.jpg")] //Random filename from saved Snapchat media
	[InlineData("652999454.jpg")] //Random name I made
	[InlineData("not a timestamp lol.jpg")] //Random name I made
	[InlineData("2022031620532000_s.mp4")] //N Switch filename
	public void FilenameTest(string filename)
	{
		_testOutputHelper.WriteLine(DateTakenExtractor.AnalyzeFilename(filename).ToString());
	}
}