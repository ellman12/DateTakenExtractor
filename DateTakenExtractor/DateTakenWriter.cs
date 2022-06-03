using System.Diagnostics;

namespace DateTakenExtractor;

//Contains static methods for writing/modifying Date Taken metadata in photos and videos.
public static partial class DateTakenExtractor
{
	///<summary></summary>
	///<param name="fullPath"></param>
	public static void ModifyDateTaken(string fullPath)
	{
		
	}

	
	public static void ModifyDateTaken(FileStream fileStream)
	{
		
	}
	
	private static void ModifyImageDateTaken(string fullPath, DateTime newDateTaken)
	{
		string DT = newDateTaken.ToString("yyyy:M:d H:mm:ss");
		
		Process process = new()
		{
			PriorityClass = ProcessPriorityClass.RealTime, //TODO: might need to lower this priority by 1 if too much for PC to handle
			StartInfo = new ProcessStartInfo
			{
				//https://exiftool.org/TagNames/EXIF.html
				FileName = "exiftool.exe",
				Arguments = $"\"{fullPath}\" -overwrite_original -DateTimeOriginal=\"{DT}\" -CreateDate=\"{DT}\" -ModifyDate=\"{DT}\" {(Path.GetExtension(fullPath).ToLower() == ".png" ? $"-PNG:CreationTime=\"{DT}\" -PNG:ModifyDate=\"{DT}\"" : "")}",
				CreateNoWindow = true,
				RedirectStandardError = false,
				RedirectStandardInput = false,
				RedirectStandardOutput = false,
				WindowStyle = ProcessWindowStyle.Hidden
			}
		};
		process.Start();
		process.WaitForExit(); //TODO: is this needed?
	}

	private static void ModifyVideoDateTaken(string fullPath, DateTime newDateTaken)
	{
	}
}