using System.Text.RegularExpressions;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;

namespace DateTakenExtractor;

///Contains static methods for making it easy to get Date Taken metadata from photos and videos.
public static class DateTakenExtractor
{
	///Used to represent where a Date Taken value came from in a file.
	public enum DateTakenSrc
	{
		///Date Taken came from file's metadata.
		Metadata, 
		
		///Date Taken came from filename.
		Filename,
		
		///No Date Taken in metadata or filename.
		None
	}

	///<summary>First tries to find Date Taken in the metadata of the file. If it can, uses that. If it can't find a DT in the metadata, looks in the filename. If no data in both, return null.</summary>
	///<param name="fullPath">The full path to the file.</param>
	///<param name="dateTakenSrc">'Metadata' if found DT in metadata. 'Filename' if DT came from filename. 'None' if no DT found and thus DT is null.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is not a valid path.</exception>
	///<exception cref="FileNotFoundException">Thrown if fullPath is a file that doesn't exist.</exception>
	///<returns>A DateTime? representing the Date Taken that was found, otherwise null.</returns>
	public static DateTime? GetDateTimeAuto(string fullPath, out DateTakenSrc dateTakenSrc)
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
		string ext = Path.GetExtension(fullPath).ToLower(); //Some files might have an extension that isn't all lowercase, like '.MOV'.
		if (ext is ".jpg" or ".jpeg" or ".png" or ".gif") dateTaken = AnalyzeExif(fullPath);
		if (ext is ".mp4" or ".mov" or ".mkv") dateTaken = AnalyzeQuickTime(fullPath);
		if (dateTaken != null) return dateTaken; //Found DT in metadata.

		dateTaken = AnalyzeFilename(Path.GetFileNameWithoutExtension(fullPath));
		return dateTaken; //← Could be null or an actual value from this ↑.
	}

	///<summary>Get Date Taken metadata from just the filename.</summary>
	///<param name="filename">The filename to analyze. You <i>can</i> also give it the full path to the file and it <i>might</i> work, but passing in just the filename is preferred.</param>
	///<exception cref="ArgumentNullException">Thrown if filename is null.</exception>
	///<returns>A DateTime? representing the Date Taken that was found in the filename, otherwise null.</returns>
	///<remarks>If you pass in a full path instead of a filename, it will attempt to strip out the extra characters and get just the filename, which is then used.</remarks>
	public static DateTime? GetDateTakenFromFilename(string filename)
	{
		if (filename == null) throw new ArgumentNullException(nameof(filename));
		if (Path.IsPathFullyQualified(filename)) Path.GetFileNameWithoutExtension(filename);
		
		DateTime? dateTaken = AnalyzeFilename(filename);
		return dateTaken;
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
		try
		{
			IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(fullPath);
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
		try
		{
			IEnumerable<MetadataExtractor.Directory> directories = QuickTimeMetadataReader.ReadMetadata(new FileStream(fullPath, FileMode.Open));
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
	
	///<summary>Analyzes a filename to see if it has a timestamp in it.</summary>
	///<param name="filename">The filename to analyze, with or without the file extension.</param>
	///<returns>A DateTime? representing the timestamp that was found in the file. null if couldn't find a timestamp.</returns>
	private static DateTime? AnalyzeFilename(string filename)
	{
		//Each thing in () is considered a group. First part is for the junk data. Adds it to a group so it stays away from the other useful groups. I think it only comes from Steam screenshots.
		//The '[-_\. ]?' handle the presence or absence of separator characters (' ', '-', '_', '.') present in most filenames, like 'IMG_20210320_175909.jpg', 'Capture 2020-12-26 21_03_05.png', and '2020-10-06_13.53.33.png'.
		const string PATTERN = @"(\d+[-_\.: ])?(\d{4})[-_\.: ]?(\d{2})[-_\.: ]?(\d{2})[-_\.: ]?(\d{2})[-_\.: ]?(\d{2})[-_\.: ]?(\d{2})";
		MatchCollection matches = new Regex(PATTERN).Matches(filename);
		if (matches.Count == 0) return null; //.Count should only ever be 0 or 1 with this pattern, since there should only ever be 0 or 1 matches.

		//groups[0] is the whole match that was returned, which may or may not contain extra junk data that isn't used, like extension, and other characters.
		//groups[1] could contain some extra junk at the start of the filename, (e.g., 105600 from Steam screenshots), so it's ignored.
		//groups[2] is the year, groups[3] is the month, etc.
		GroupCollection groups = matches[0].Groups;
		return DateTime.Parse($"{groups[2]}-{groups[3]}-{groups[4]} {groups[5]}:{groups[6]}:{groups[7]}");
	}

	///<summary>Take a timestamp string like '2018-11-03 07:26:12', or of similar format, and attempt to parse and return a DateTime representing it.</summary>
	///<param name="timestamp">The timestamp to attempt to parse.</param>
	///<returns>A DateTime? representing the parsed timestamp. null if couldn't determine Date Taken.</returns>
	private static DateTime? Parse(string timestamp)
	{
		try
		{
			//Remove any useless characters, and turn all the parts of the timestamp into integers for creating the DateTime.
			timestamp = timestamp.Replace(" ", "").Replace("-", "").Replace("_", "").Replace(".", "").Replace(":", "").Replace(";", "");
			int year = Int32.Parse(timestamp[..4]);
			int month = Int32.Parse(timestamp[4..6]);
			int day = Int32.Parse(timestamp[6..8]);
			int hour = Int32.Parse(timestamp[8..10]);
			int min = Int32.Parse(timestamp[10..12]);
			int sec = Int32.Parse(timestamp[12..14]);
			return new DateTime(year, month, day, hour, min, sec);
		}
		catch (FormatException)
		{
			return null;
		}
	}
}