using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;

namespace DateTakenExtractor;

///Contains static methods for making it easy to get Date Taken metadata from photos and videos.
//This file contains methods that accept FileStreams as arguments.
public static partial class DateTakenExtractor
{
	///<summary>First tries to find Date Taken in the metadata of the file. If it can, uses that. If it can't find a DT in the metadata, looks in the filename. If no data in both, return null.</summary>
	///<param name="fileStream">FileStream to analyze.</param>
	///<param name="dateTakenSrc">'Metadata' if found DT in metadata. 'Filename' if DT came from filename. 'None' if no DT found and thus DT is null.</param>
	///<exception cref="ArgumentNullException">Thrown if fileStream is null.</exception>
	///<returns>A DateTime? representing the Date Taken that was found in the metadata, otherwise null.</returns>
	///<remarks>This method will not close or dispose the FileStream that is passed in.</remarks>
	public static DateTime? GetDateTakenAuto(FileStream fileStream, out DateTakenSrc dateTakenSrc)
	{
		DateTime? result = GetDateTakenFromMetadata(fileStream);
		if (result != null)
		{
			dateTakenSrc = DateTakenSrc.Metadata;
			return result;
		}
		
		result = GetDateTakenFromFilename(fileStream.Name);
		if (result != null)
		{
			dateTakenSrc = DateTakenSrc.Filename;
			return result;
		}

		dateTakenSrc = DateTakenSrc.None;
		return null;
	}
	
	///<summary>Attempt to get Date Taken metadata from just the file's internal metadata.</summary>
	///<param name="fileStream">FileStream to analyze.</param>
	///<exception cref="ArgumentNullException">Thrown if fileStream is null.</exception>
	///<returns>A DateTime? representing the Date Taken that was found in the metadata, otherwise null.</returns>
	///<remarks>This method will not close or dispose the FileStream that is passed in.</remarks>
	public static DateTime? GetDateTakenFromMetadata(FileStream fileStream)
	{
		if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));
		
		DateTime? dateTaken = null;
		string ext = Path.GetExtension(fileStream.Name).ToLower(); //Some files might have an extension that isn't all lowercase, like '.MOV'.
		if (ext is ".jpg" or ".jpeg" or ".png" or ".gif") dateTaken = AnalyzeExif(fileStream);
		else if (ext is ".mp4" or ".mov" or ".mkv") dateTaken = AnalyzeQuickTime(fileStream);
		return dateTaken; //← Could be null or an actual value from this ↑.
	}
	
	///<summary>Get Date Taken from both metadata AND the filename, when possible.</summary>
	///<param name="fileStream">FileStream to analyze.</param>
	///<param name="metadataDT">The DateTime? variable to store the metadata Date Taken in.</param>
	///<param name="filenameDT">The DateTime? variable to store the filename Date Taken in.</param>
	///<exception cref="ArgumentNullException">Thrown if fileStream is null.</exception>
	///<remarks>This method will not close or dispose the FileStream that is passed in.</remarks>
	public static void GetDateTakenFromBoth(FileStream fileStream, out DateTime? metadataDT, out DateTime? filenameDT)
	{
		metadataDT = GetDateTakenFromMetadata(fileStream);
		filenameDT = GetDateTakenFromFilename(fileStream.Name);
	}
	
	///<summary>Analyzes the Exif metadata (if any) of an image file.</summary>
	///<param name="fileStream">FileStream to analyze.</param>
	///<returns>A DateTime? representing the Date Taken metadata that was found in the file. null if couldn't find any data.</returns>
	///<remarks>This method will not close or dispose the FileStream that is passed in.</remarks>
	private static DateTime? AnalyzeExif(FileStream fileStream)
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
	
	///<summary>Analyzes the QuickTime metadata (if any) of a video file.</summary>
	///<param name="fileStream">FileStream to analyze.</param>
	///<returns>A DateTime? representing the Date Taken metadata that was found in the file. null if couldn't find any data.</returns>
	///<remarks>This method will not close or dispose the FileStream that is passed in.</remarks>
	private static DateTime? AnalyzeQuickTime(FileStream fileStream)
	{
		try
		{
			IEnumerable<MetadataExtractor.Directory> directories = QuickTimeMetadataReader.ReadMetadata(fileStream);
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