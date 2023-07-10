using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;

namespace DateTakenExtractor;

//This file contains methods that accept direct file paths as arguments.
public static partial class DateTakenExtractor
{
	///<summary>First tries to find Date Taken in the metadata of the file. If it can, uses that. If it can't find a DT in the metadata, looks in the filename. If no data in both, return null.</summary>
	///<param name="fullPath">The full path to the file.</param>
	///<param name="dateTakenSrc">'Metadata' if found DT in metadata. 'Filename' if DT came from filename. 'None' if no DT found and thus DT is null.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is not a valid path.</exception>
	///<exception cref="FileNotFoundException">Thrown if fullPath is a file that doesn't exist.</exception>
	///<returns>A DateTime? representing the Date Taken that was found, otherwise null.</returns>
	public static DateTime? GetDateTakenAuto(string fullPath, out DateTakenSrc dateTakenSrc)
	{
		DateTime? result = GetDateTakenFromMetadata(fullPath);
		if (result != null)
		{
			dateTakenSrc = DateTakenSrc.Metadata;
			return result;
		}
		
		result = GetDateTakenFromFilename(fullPath);
		if (result != null)
		{
			dateTakenSrc = DateTakenSrc.Filename;
			return result;
		}

		dateTakenSrc = DateTakenSrc.None;
		return null;
	}
	
	///<summary>Attempt to get Date Taken metadata from just the file's internal metadata.</summary>
	///<param name="fullPath">The full path to the file.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is not a valid path.</exception>
	///<exception cref="FileNotFoundException">Thrown if fullPath is a file that doesn't exist.</exception>
	///<returns>A DateTime? representing the Date Taken that was found in the metadata, otherwise null.</returns>
	public static DateTime? GetDateTakenFromMetadata(string fullPath)
	{
		if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
		if (!Path.IsPathFullyQualified(fullPath)) throw new ArgumentException($"{fullPath} is not a valid path.");
		if (!File.Exists(fullPath)) throw new FileNotFoundException($"{fullPath} does not exist.");
		
		DateTime? dateTaken = null;
		string ext = Path.GetExtension(fullPath).ToLower();

		if (ext == ".png") dateTaken = GetPngDateTaken(fullPath);
		if (dateTaken != null) return dateTaken; //If it got DT from the PNG file's metadata, continue. Otherwise keep looking.

		if (dateTaken == null && IsPhotoExt(ext)) dateTaken = AnalyzeExif(fullPath);
		else if (IsVideoExt(ext)) dateTaken = AnalyzeQuickTime(fullPath);
		return dateTaken; //← Could be null or an actual value from ↑ this.
	}

	///<summary>Get Date Taken from both metadata AND the filename, when possible.</summary>
	///<param name="fullPath">The full path to the file.</param>
	///<param name="metadataDT">The DateTime? variable to store the metadata Date Taken in.</param>
	///<param name="filenameDT">The DateTime? variable to store the filename Date Taken in.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is not a valid path.</exception>
	///<exception cref="FileNotFoundException">Thrown if fullPath is a file that doesn't exist.</exception>
	public static void GetDateTakenFromBoth(string fullPath, out DateTime? metadataDT, out DateTime? filenameDT)
	{
		metadataDT = GetDateTakenFromMetadata(fullPath);
		filenameDT = GetDateTakenFromFilename(fullPath);
	}

	///<summary>Analyzes the Exif metadata (if any) of an image file.</summary>
	///<param name="fullPath">Full path to the item to analyze.</param>
	///<returns>A DateTime? representing the Date Taken metadata that was found in the file. null if couldn't find any data.</returns>
	private static DateTime? AnalyzeExif(string fullPath)
	{
		using FileStream fileStream = new(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
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
	///<param name="fullPath">Full path to the item to analyze.</param>
	///<returns>A DateTime? representing the Date Taken metadata that was found in the file. null if couldn't find any data.</returns>
	private static DateTime? AnalyzeQuickTime(string fullPath)
	{
		using FileStream fileStream = new(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
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