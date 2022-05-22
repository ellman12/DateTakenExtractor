using System.Text.RegularExpressions;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.QuickTime;

namespace Date_Taken_Extractor;

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

	/*
	///<summary>First tries to find Date Taken in the metadata of the file. If it can, uses that. If it can't, looks in the filename. If no data in both, return null.</summary>
	/// <param name="fullPath">The full path to the file.</param>
	/// <param name="dateTaken">The DateTime? variable to store the Date Taken in.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is a file that doesn't exist.</exception>
	/// <returns>DateTakenSrc enum representing where the Date Taken came from.</returns>
	public static DateTakenSrc GetDateTimeAuto(string fullPath, out DateTime? dateTaken)
	{
		if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
		if (!File.Exists(fullPath)) throw new ArgumentException("File specified does not exist.");
	}
	
	///<summary>Date Taken metadata from just the file's internal metadata.</summary>
	///<param name="fullPath">The full path to the file.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is a file that doesn't exist.</exception>
	///<returns>The Date Taken that was found in the metadata, otherwise null.</returns>
	public static DateTime? GetDateTakenFromMetadata(string fullPath)
	{
		if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
		if (!File.Exists(fullPath)) throw new ArgumentException("File specified does not exist.");
	}

	///<summary>Get Date Taken metadata from just the filename.</summary>
	///<param name="fullPath">The full path to the file.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is a file that doesn't exist.</exception>
	///<returns>The Date Taken that was found in the filename, otherwise null.</returns>
	public static DateTime? GetDateTakenFromFilename(string fullPath)
	{
		if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
		if (!File.Exists(fullPath)) throw new ArgumentException("File specified does not exist.");
	}
	
	///<summary>Get Date Taken from metadata AND the filename.</summary>
	///<param name="fullPath">The full path to the file.</param>
	///<param name="metadataDT">The DateTime? variable to store the metadata Date Taken in.</param>
	///<param name="filenameDT">The DateTime? variable to store the filename Date Taken in.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is a file that doesn't exist.</exception>
	public static void GetDateTakenFromBoth(string fullPath, out DateTime? metadataDT, out DateTime? filenameDT)
	{
		if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
		if (!File.Exists(fullPath)) throw new ArgumentException("File specified does not exist.");
	}
	*/

	///<summary>Analyzes the Exif metadata (if any) of a (usually image) file.</summary>
	///<param name="fullPath">Full path to the item to analyze.</param>
	///<returns>A DateTime? representing the Date Taken metadata that was found in the file. null if couldn't find any data.</returns>
	public static DateTime? AnalyzeExif(string fullPath)
    {
	    try
	    {
		    IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(fullPath);
		    ExifSubIfdDirectory subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().First();

		    //Check at most three different places for possible DT Exif metadata.
		    string? dtDigitized = subIfdDirectory.GetDescription(ExifDirectoryBase.TagDateTimeDigitized);
		    if (dtDigitized != null) return Parse(dtDigitized);

		    //In testing, this tag (I think) always had the same value as Digitized, but including it anyways just in case.
		    string? dtOriginal = subIfdDirectory.GetDescription(ExifDirectoryBase.TagDateTimeOriginal);
		    if (dtOriginal != null) return Parse(dtOriginal);

		    //In testing, this tag never had data but including it anyways just in case.
		    string? dt = subIfdDirectory.GetDescription(ExifDirectoryBase.TagDateTime);
		    if (dt != null) return Parse(dt);
	    }
	    catch (Exception e) when (e is UnauthorizedAccessException or InvalidOperationException) //InvalidOp can occur when file has no DT metadata.
	    {
			return null; //No DT metadata in file.
	    }
		return null; //No DT metadata in file.
    }

	///<summary>Analyzes the QuickTime metadata (if any) of a (usually video) file.</summary>
	///<param name="fullPath">Full path to the item to analyze.</param>
	/// <param name="dateTakenSrc">If this file had DT metadata, dateTakenSrc is set to 'Metadata'. If it doesn't, it's set to 'None'.</param>
	///<returns>A DateTime? representing the Date Taken metadata that was found in the file. null if couldn't find any data.</returns>
	public static DateTime? AnalyzeQuickTime(string fullPath, out DateTakenSrc dateTakenSrc)
	{
		try
		{
			IEnumerable<MetadataExtractor.Directory> directories = QuickTimeMetadataReader.ReadMetadata(new FileStream(fullPath, FileMode.Open));
			QuickTimeMovieHeaderDirectory directory = directories.OfType<QuickTimeMovieHeaderDirectory>().First();

			if (directory.TryGetDateTime(QuickTimeMovieHeaderDirectory.TagCreated, out DateTime dateTaken)) //If it found DT metadata, return that value.
			{
				dateTakenSrc = DateTakenSrc.Metadata;
				return dateTaken;
			}

			dateTakenSrc = DateTakenSrc.None;
			return null; //No DT metadata in file.
		}
		catch (Exception e) when (e is UnauthorizedAccessException or InvalidOperationException) //In testing, UnauthorizedAccessExceptions only happened for Switch clips. InvalidOperationExceptions can happen when no metadata in file.
		{
			dateTakenSrc = DateTakenSrc.None;
			return null; //No DT metadata in file.
		}
	}
	
	///<summary>Analyzes a filename to see if it has a timestamp in it.</summary>
	///<param name="filename">The filename to analyze, with or without the file extension.</param>
	///<returns>A DateTime? representing the timestamp that was found in the file. null if couldn't find a timestamp.</returns>
	public static DateTime? AnalyzeFilename(string filename)
	{
		//Each thing in () is considered a group. First part is for the junk data. Adds it to a group so it stays away from the other useful groups. I think it only comes from Steam screenshots.
		//The '[-_\. ]?' handle the presence or absence of separator characters (' ', '-', '_', '.') present in most filenames, like 'IMG_20210320_175909.jpg', 'Capture 2020-12-26 21_03_05.png', and '2020-10-06_13.53.33.png'.
		const string PATTERN = @"(\d+[-_ ])?(\d{4})[-_\. ]?(\d{2})[-_\. ]?(\d{2})[-_\. ]?(\d{2})[-_\. ]?(\d{2})[-_\. ]?(\d{2})";
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
	public static DateTime? Parse(string timestamp)
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