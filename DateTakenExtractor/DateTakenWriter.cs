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
	
	private static void ModifyExifDT()
	{
		//TODO: experiment with wrappers for exiftool and ffmpeg. Look over all exif/ffmpeg command line arguments
		//TODO: https://exiftool.org/exiftool_pod.html
		/*
		 * Arguments to try:
		 * -TAG or --TAG                    Extract or exclude specified tag
		 *  -TAG[+-^]=[VALUE]                Write new value for tag
		 *   -d FMT      (-dateFormat)        Set format for date/time values
		 * -s[NUM]     (-short)             Short output format
  		 * -S          (-veryShort)         Very short output format
  		 * -v[NUM]     (-verbose)           Print verbose messages
  		 * -fast[NUM]                       Increase speed when extracting metadata
  		 * -o OUTFILE  (-out)               Set output file or directory name
  		 * -overwrite_original              Overwrite original by renaming tmp file
  		 * -overwrite_original_in_place     Overwrite original by copying tmp file
  		 * -P          (-preserve)          Preserve file modification date/time
		 *   -q          (-quiet)             Quiet processing
		 * -wm MODE    (-writeMode)         Set mode for writing/creating tags
		 *   -list[w|f|wf|g[NUM]|d|x]         List various exiftool capabilities
		 * -api OPT[[^]=[VAL]]              Set ExifTool API option
		 *  -common_args                     Define common arguments
		 *  -execute[NUM]                    Execute multiple commands on one line
		 *   -stay_open FLAG                  Keep reading -@ argfile even after EOF
		 *  -userParam PARAM[[^]=[VAL]]      Set user parameter (API UserParam opt)
		 */
		Process process = new()
		{
			PriorityClass = ProcessPriorityClass.RealTime,
			StartInfo = new ProcessStartInfo
			{
				Arguments = null,
				CreateNoWindow = true,
				FileName = "exiftool",
				RedirectStandardError = false,
				RedirectStandardInput = false,
				RedirectStandardOutput = false,
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = null //TODO
			}
		};
		process.Start();
	}

	private static void ModifyVideoDT()
	{
		Process process = new()
		{
			PriorityClass = ProcessPriorityClass.RealTime,
			StartInfo = new ProcessStartInfo
			{
				Arguments = null,
				CreateNoWindow = true,
				FileName = "exiftool",
				RedirectStandardError = false,
				RedirectStandardInput = false,
				RedirectStandardOutput = false,
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = null //TODO
			}
		};
		process.Start();
	}
}