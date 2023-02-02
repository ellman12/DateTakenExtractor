using System.Diagnostics;
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

	///<summary>Determine if the specified extension is a supported photo extension. The supported extensions are ".jpg", ".jpeg", ".png", and ".gif".</summary>
	///<param name="ext">The extension to analyze.</param>
	///<returns>True if the extension was an photo extension, false otherwise.</returns>
	public static bool IsPhotoExt(string ext) => ext.ToLower() is ".jpg" or ".jpeg" or ".png" or ".gif";
	
	///<summary>Determine if the specified extension is a supported video extension. The supported extensions are ".mp4", ".mov", and ".mkv".</summary>
	///<param name="ext">The extension to analyze.</param>
	///<returns>True if the extension was a video extension, false otherwise.</returns>
	public static bool IsVideoExt(string ext) => ext.ToLower() is ".mp4" or ".mov" or ".mkv";

	///<summary>Get Date Taken metadata from just the filename of a file.</summary>
	///<param name="filename">The filename to analyze. You <i>can</i> also give it the full path to the file and it <i>might</i> work, but passing in just the filename is preferred.</param>
	///<exception cref="ArgumentNullException">Thrown if filename is null.</exception>
	///<returns>A DateTime? representing the Date Taken that was found in the filename, otherwise null.</returns>
	///<remarks>If you pass in a full path instead of a filename, it will attempt to strip out the extra characters and get just the filename, which is then used.</remarks>
	public static DateTime? GetDateTakenFromFilename(string filename)
	{
		if (String.IsNullOrWhiteSpace(filename)) throw new ArgumentNullException(nameof(filename));
		if (Path.IsPathFullyQualified(filename)) filename = Path.GetFileNameWithoutExtension(filename);
		return AnalyzeFilename(filename);
	}

	/// <summary>Using ExifTool, attempt to get this PNG file's Date Taken.</summary>
	/// <param name="fullPath">The full path to the PNG.</param>
	/// <returns>Either null or a Date Taken value.</returns>
	/// <remarks>PNGs are very strange and usually don't have Date Taken, but sometimes they can.</remarks>
	private static DateTime? GetPngDateTakenExifTool(string fullPath)
	{
		Process p = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "exiftool",
				Arguments = $"\"{fullPath}\" -T -PNG:CreationTime",
				CreateNoWindow = true,
				RedirectStandardInput = true,
				//1/5/2023: Set this ↑ to true because for some reason when using this in PSS's Import page, having this set to false caused the ExifTool process to not really be able
				//to finish sometimes/all time time (I think all the time). This only happened if I used GetDateTakenFromBoth instead of GetDateTakenAuto. Auto was used in UploadApply.
				RedirectStandardOutput = true, //Necessary for reading output of ExifTool.
				WindowStyle = ProcessWindowStyle.Hidden
			}
		};
		p.Start();
		p.WaitForExit();
		string? output = p.StandardOutput.ReadLine();
		return output == null ? null : Parse(output);
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
		
		try { return DateTime.Parse($"{groups[2]}-{groups[3]}-{groups[4]} {groups[5]}:{groups[6]}:{groups[7]}"); }
		catch (FormatException) { return null; }
	}
	
	///<summary>Take a timestamp string like '2018-11-03 07:26:12', '20181103072612', or of similar format, and attempt to parse and return a DateTime representing it.</summary>
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
		catch (Exception)
		{
			return null;
		}
	}
}