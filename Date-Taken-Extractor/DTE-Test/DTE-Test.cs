using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

	[Fact]
	public void PngTest()
	{
		// string[] pngs = System.IO.Directory.GetFiles("C:/Users/Elliott/Videos/test", "*.png", SearchOption.AllDirectories);
		//
		// foreach (string png in pngs)
		// {
		// 	_testOutputHelper.WriteLine(png);
		// 	IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(png);
		// 	foreach (Directory directory in directories)
		// 	foreach (Tag tag in directory.Tags)
		// 		_testOutputHelper.WriteLine($"{directory.Name} - {tag.Name} = {tag.Description}");
		//
		// 	_testOutputHelper.WriteLine("\n");
		// }
	}
}