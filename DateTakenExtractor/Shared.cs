using System.Text.RegularExpressions;

namespace DateTakenExtractor;

///Contains static methods for making it easy to get Date Taken metadata from photos and videos.
//This file contains shared stuff between the files.
public static partial class DateTakenExtractor
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
}