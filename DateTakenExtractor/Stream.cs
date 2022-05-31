using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;

namespace DateTakenExtractor;

///Contains static methods for making it easy to get Date Taken metadata from photos and videos.
//This file contains methods that accept Streams as arguments.
public static partial class DateTakenExtractor
{
	///<summary>Analyzes the Exif metadata (if any) of an image file.</summary>
	///<param name="stream">Stream to analyze.</param>
	///<returns>A DateTime? representing the Date Taken metadata that was found in the file. null if couldn't find any data.</returns>
	///<remarks>This method will not close or dispose the Stream that is passed in.</remarks>
	private static DateTime? AnalyzeExif(Stream stream)
	{
		try
		{
			IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(stream);
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
	
	///<summary>Analyzes the QuickTime metadata (if any) of a video file.</summary>
	///<param name="stream">Stream to analyze.</param>
	///<returns>A DateTime? representing the Date Taken metadata that was found in the file. null if couldn't find any data.</returns>
	///<remarks>This method will not close or dispose the Stream that is passed in.</remarks>
	private static DateTime? AnalyzeQuickTime(Stream stream)
	{
		try
		{
			IEnumerable<MetadataExtractor.Directory> directories = QuickTimeMetadataReader.ReadMetadata(stream);
			QuickTimeMovieHeaderDirectory directory = directories.OfType<QuickTimeMovieHeaderDirectory>().First();

			if (directory.TryGetDateTime(QuickTimeMovieHeaderDirectory.TagCreated, out DateTime dateTaken)) //If it found DT metadata, return that value.
				return dateTaken;

			return null; //No DT metadata in file.
		}
		catch (Exception e) when (e is UnauthorizedAccessException or InvalidOperationException) //In testing, UnauthorizedAccessExceptions only happened for Switch clips. InvalidOperationExceptions can happen when no metadata in file.
		{
			return null;
		}
	}
}