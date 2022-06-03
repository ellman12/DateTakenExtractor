using System.Diagnostics;

namespace DateTakenExtractor;

//Contains static methods for writing/modifying Date Taken metadata in photos and videos.
public static partial class DateTakenExtractor
{
	///<summary></summary>
	///<param name="fullPath"></param>
	public static void UpdateDateTaken(string fullPath)
	{
		
	}

	
	public static void UpdateDateTaken(FileStream fileStream)
	{
		
	}
	
	private static void UpdateImageDateTaken(string fullPath, DateTime newDateTaken)
	{
		string DT = newDateTaken.ToString("yyyy:M:d H:mm:ss");
		
		using Process process = new()
		{
			StartInfo = new ProcessStartInfo
			{
				//https://exiftool.org/TagNames/EXIF.html
				//https://exiftool.org/TagNames/Shortcuts.html
				FileName = "exiftool",
				Arguments = $"\"{fullPath}\" -overwrite_original -AllDates=\"{DT}\" {(Path.GetExtension(fullPath).ToLower() == ".png" ? $"-PNG:CreationTime=\"{DT}\" -PNG:ModifyDate=\"{DT}\"" : "")}",
				CreateNoWindow = true,
				RedirectStandardError = false,
				RedirectStandardInput = false,
				RedirectStandardOutput = false,
				WindowStyle = ProcessWindowStyle.Hidden
			}
		};
		process.Start();
	}

	private static void UpdateVideoDateTaken(string fullPath, DateTime newDateTaken)
	{
		string DT = newDateTaken.ToString("yyyy:M:d H:mm:ss");
		
		using Process process = new()
		{
			StartInfo = new ProcessStartInfo
			{
				//https://exiftool.org/forum/index.php?topic=11100.msg59329#msg59329
				FileName = "exiftool",
				Arguments = $"\"{fullPath}\" -overwrite_original - -CreateDate=\"{DT}\" -ModifyDate=\"{DT}\" -Track*Date=\"{DT}\" -Media*Date=\"{DT}\" Quicktime:DateTimeOriginal=\"{DT}\"",
				CreateNoWindow = true,
				RedirectStandardError = false,
				RedirectStandardInput = false,
				RedirectStandardOutput = false,
				WindowStyle = ProcessWindowStyle.Hidden
			}
		};
		process.Start();
	}
}