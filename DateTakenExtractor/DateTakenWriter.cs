using System.Diagnostics;

namespace DateTakenExtractor;

//Contains static methods for writing/modifying Date Taken metadata in photos and videos.
public static partial class DateTakenExtractor
{
	///<summary>Uses ExifTool to update the internal Date Taken metadata of the specified photo or video.</summary>
	///<param name="fullPath">The full path to the item to update</param>
	///<param name="newDateTaken">The new Date Taken to embed in the file's metadata. If null, removes any Date Taken metadata already present.</param>
	public static void UpdateDateTaken(string fullPath, DateTime? newDateTaken)
	{
		string ext = Path.GetExtension(fullPath).ToLower();
		if (IsPhotoExt(ext)) UpdatePhotoDateTaken(fullPath, newDateTaken);
		else if (IsVideoExt(ext)) UpdateVideoDateTaken(fullPath, newDateTaken);
	}

	///<summary>Uses ExifTool to update the internal Date Taken metadata of the specified photo.</summary>
	///<param name="fullPath">The full path to the item to update.</param>
	///<param name="newDateTaken">The new Date Taken to embed in the file's metadata. If null, removes any Date Taken metadata already present.</param>
	private static void UpdatePhotoDateTaken(string fullPath, DateTime? newDateTaken)
	{
		string DT = FormatDateTaken(newDateTaken);

		using Process process = new();
		process.StartInfo = new ProcessStartInfo
		{
			//https://exiftool.org/TagNames/EXIF.html
			//https://exiftool.org/TagNames/Shortcuts.html
			FileName = "exiftool",
			Arguments = $"\"{fullPath}\" -overwrite_original -AllDates={DT} -FileAccessDate={DT} -FileCreateDate={DT} -FileModifyDate={DT} {(Path.GetExtension(fullPath).ToLower() == ".png" ? $"-PNG:CreationTime={DT} -PNG:ModifyDate={DT}" : "")}",
			CreateNoWindow = true,
			RedirectStandardError = false,
			RedirectStandardInput = false,
			RedirectStandardOutput = false,
			WindowStyle = ProcessWindowStyle.Hidden
		};
		process.Start();
		process.WaitForExit();
	}

	///<summary>Uses ExifTool to update the internal Date Taken metadata of the specified video.</summary>
	///<param name="fullPath">The full path to the item to update.</param>
	///<param name="newDateTaken">The new Date Taken to embed in the file's metadata. If null, removes any Date Taken metadata already present.</param>
	private static void UpdateVideoDateTaken(string fullPath, DateTime? newDateTaken)
	{
		//Videos require DateTime to be in UTC for some reason: https://exiftool.org/forum/index.php?PHPSESSID=a68f2cbabc087b534d7ac88e55fb932d&topic=11880.msg64084#msg64084
		string DT = FormatDateTaken(newDateTaken);

		using Process process = new();
		process.StartInfo = new ProcessStartInfo
		{
			//https://exiftool.org/forum/index.php?topic=11100.msg59329#msg59329
			//https://exiftool.org/forum/index.php?topic=11272
			FileName = "exiftool",
			Arguments = $"\"{fullPath}\" -overwrite_original -CreateDate={DT} -ModifyDate={DT} -Track*Date={DT} -Media*Date={DT} -Quicktime:DateTimeOriginal={DT} -FileAccessDate={DT} -FileCreateDate={DT} -FileModifyDate={DT}",
			CreateNoWindow = true,
			RedirectStandardError = false,
			RedirectStandardInput = false,
			RedirectStandardOutput = false,
			WindowStyle = ProcessWindowStyle.Hidden
		};
		process.Start();
		process.WaitForExit();
	}

	private static string FormatDateTaken(DateTime? dateTaken)
	{
		string? DT = dateTaken?.ToString("yyyy:M:d H:mm:ss");
		return DT == null ? "" : $"\"{DT}\"";
	}
}