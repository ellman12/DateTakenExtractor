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
	
	[Theory]
	[InlineData("2018/3 March/4/2018030420412600_s.jpg")]
	[InlineData("2018/3 March/10/2018031020470000_s.jpg")]
	[InlineData("2018/11 November/3/20181103_072612.jpg")]
	[InlineData("2021/4 April/26/IMG_20210426_145048.jpg")]
	[InlineData("2021/4 April/13/IMG_20210413_101850.jpg")]
	[InlineData("Unknown Date Taken/tempFileForShare_20220418-191816.jpg")]
	//Ones that shouldn't have any
	[InlineData("2022/3 March/16/2022031620491000_s.mp4")]
	[InlineData("2021/4 April/29/VID_20210429_160512.mp4")]
	[InlineData("Unknown Date Taken/3ujy8xj4rwl41.png")]
	[InlineData("Unknown Date Taken/9apf2enw6uy71.png")]
	[InlineData("Unknown Date Taken/FB_IMG_1641842532651.jpg")]
	[InlineData("Unknown Date Taken/botw.png")]
	[InlineData("Unknown Date Taken/image0-1.jpg")]
	[InlineData("Unknown Date Taken/thumb_1625481238683-1293588774.jpg")]
	[InlineData("Unknown Date Taken/messages_0.jpeg")]
	[InlineData("Unknown Date Taken/usw7p5qwzld71.jpg")]
	//Common filename timestamp formats I've encountered, with a few non-timestamps.
	public void ExifTest(string fullPath)
	{
		fullPath = Path.Combine("D:/My Backups/Sorted Pics and Vids From Phone, Switch, and Elsewhere 5-17-2022/", fullPath);
		DateTime? result = DateTakenExtractor.AnalyzeExif(fullPath);
		_testOutputHelper.WriteLine(result == null ? "null" : result.ToString());
	}

	[Theory]
	[InlineData("2018-11-03 07-26-12")]
	[InlineData("2018:11:03 07:26:12")]
	[InlineData("20181103072612")]
	[InlineData("2018 11 03 07 26 12")]
	[InlineData("2018-11-03 07:26:12")]
	public void ParseTest(string timestamp)
	{
		DateTime? result = DateTakenExtractor.Parse(timestamp);
		_testOutputHelper.WriteLine(result == null ? "null" : result.ToString());
	}

	[Theory]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/DSC_6663.MOV")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/IMG_0122.MOV")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/IMG_0841.MOV")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/IMG_1027.MOV")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/IMG_4418.mov")]
	[InlineData("C:/Users/Elliott/Videos/test/Photos-001/TWUC6365.MOV")]
	[InlineData("C:/Users/Elliott/Videos/test/20210501_193046.mp4")]
	[InlineData("D:/My Backups/Sorted Pics and Vids From Phone, Switch, and Elsewhere 5-17-2022/2018/3 March/17/2018031720595600_s.mp4")]
	[InlineData("D:/My Backups/Sorted Pics and Vids From Phone, Switch, and Elsewhere 5-17-2022/2022/4 April/17/VID_20220417_085545.mp4")]
	[InlineData("D:/My Backups/Sorted Pics and Vids From Phone, Switch, and Elsewhere 5-17-2022/2022/4 April/26/VID_20220426_085300.mp4")]
	[InlineData("D:/My Backups/Sorted Pics and Vids From Phone, Switch, and Elsewhere 5-17-2022/2022/5 May/10/VID_20220510_173424.mp4")]
	[InlineData("D:/My Backups/Sorted Pics and Vids From Phone, Switch, and Elsewhere 5-17-2022/2022/1 January/1/2022010120534400_s.mp4")]
	public void QuickTimeTest(string fullPath)
	{
		_testOutputHelper.WriteLine(fullPath);
		try
		{
			IEnumerable<MetadataExtractor.Directory> directories = QuickTimeMetadataReader.ReadMetadata(new FileStream(fullPath, FileMode.Open));
			QuickTimeMovieHeaderDirectory directory = directories.OfType<QuickTimeMovieHeaderDirectory>().First();

			if (directory.TryGetDateTime(QuickTimeMovieHeaderDirectory.TagCreated, out DateTime dateTaken)) //If it found DT metadata, return that value.
			{
				_testOutputHelper.WriteLine(dateTaken.ToString());
				return;
			}
			
			_testOutputHelper.WriteLine("null");
		}
		catch (UnauthorizedAccessException) //In testing, this only happened for Switch clips.
		{
			_testOutputHelper.WriteLine("null");
		}
	}
}