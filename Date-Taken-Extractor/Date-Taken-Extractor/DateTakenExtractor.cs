using System.Globalization;
using ExifLib;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Directory = MetadataExtractor.Directory;
using ExifReader = MetadataExtractor.Formats.Exif.ExifReader;

namespace Date_Taken_Extractor;

public static class DateTakenExtractor
{
	private const string TEST_PATH = "C:/Users/Elliott/Videos/Photos-001/IMG_20210426_145048.jpg";
	
	public static string MD()
	{
		IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(TEST_PATH);
		ExifSubIfdDirectory? subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
		string? dateTime = subIfdDirectory!.GetDescription(ExifDirectoryBase.TagDateTime);
		if (dateTime == null)
			return "null";
		return dateTime;
	}

	public static string Exif()
	{
		ExifLib.ExifReader reader = new(TEST_PATH);
		reader.GetTagValue(ExifTags.DateTimeDigitized, out DateTime dateTaken);
		return dateTaken.ToString(CultureInfo.InvariantCulture);
	}
}