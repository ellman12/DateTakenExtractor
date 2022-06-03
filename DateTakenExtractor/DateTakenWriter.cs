using System.Diagnostics;

namespace DateTakenExtractor;

//Contains static methods for writing/modifying Date Taken metadata in photos and videos.
public static partial class DateTakenExtractor
{
	///<summary>Uses ExifTool to update the internal Date Taken metadata of the specified photo or video.</summary>
	///<param name="fullPath">The full path to the item to update</param>
	///<param name="newDateTaken">The new Date Taken to embed in the file's metadata.</param>
	public static void UpdateDateTaken(string fullPath, DateTime newDateTaken)
	{
		using FileStream fileStream = new(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None); //TODO: this .None option might cause problems...
		string ext = Path.GetExtension(fileStream.Name).ToLower();
		if (IsPhotoExt(ext)) UpdatePhotoDateTaken(fileStream.Name, newDateTaken);
		else if (IsVideoExt(ext)) UpdateVideoDateTaken(fileStream.Name, newDateTaken);
	}
	
	///<summary>Uses ExifTool to update the internal Date Taken metadata of the specified photo or video.</summary>
	/// <param name="fileStream">FileStream of the item to update.</param>
	///<param name="newDateTaken">The new Date Taken to embed in the file's metadata.</param>
	public static void UpdateDateTaken(FileStream fileStream, DateTime newDateTaken)
	{
		string ext = Path.GetExtension(fileStream.Name).ToLower();
		if (IsPhotoExt(ext)) UpdatePhotoDateTaken(fileStream.Name, newDateTaken);
		else if (IsVideoExt(ext)) UpdateVideoDateTaken(fileStream.Name, newDateTaken);
	}
	
	///<summary>Uses ExifTool to update the internal Date Taken metadata of the specified photo.</summary>
	///<param name="fullPath">The full path to the item to update.</param>
	///<param name="newDateTaken">The new Date Taken to embed in the file's metadata.</param>
	private static void UpdatePhotoDateTaken(string fullPath, DateTime newDateTaken)
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

	///<summary>Uses ExifTool to update the internal Date Taken metadata of the specified video.</summary>
	///<param name="fullPath">The full path to the item to update.</param>
	///<param name="newDateTaken">The new Date Taken to embed in the file's metadata.</param>
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