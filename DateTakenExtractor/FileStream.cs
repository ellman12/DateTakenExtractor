using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace DateTakenExtractor;

///Contains static methods for making it easy to get Date Taken metadata from photos and videos.
//This file contains methods that accept FileStreams as arguments.
public static partial class DateTakenExtractor
{
	///<summary>Analyzes the Exif metadata (if any) of an image file.</summary>
	///<param name="fileStream">FileStream of the file to analyze.</param>
	///<returns>A DateTime? representing the Date Taken metadata that was found in the file. null if couldn't find any data.</returns>
	private static DateTime? AnalyzeExif(FileStream fileStream)
	{
		using (fileStream)
		{
			try
			{
				IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(fileStream);
				ExifSubIfdDirectory directory = directories.OfType<ExifSubIfdDirectory>().First();

				//Check at most three different places for possible DT Exif metadata.
				if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeDigitized, out DateTime result)) return result;

				//In testing, this tag (I think) always had the same value as Digitized, but including it anyways just in case.
				if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out result)) return result;

				//In testing, this tag never had data but including it anyways just in case.
				if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTime, out result)) return result;

				return null; //No DT metadata in file.
			}
			catch (Exception e) when (e is UnauthorizedAccessException or InvalidOperationException) //InvalidOp can occur when file has no DT metadata.
			{
				return null;
			}
		}
	}
}