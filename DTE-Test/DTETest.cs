using System;
using System.Diagnostics;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace DTE_Test;

public class DTETest
{
	private readonly ITestOutputHelper _testOutputHelper;

	public DTETest(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

	[Theory]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2022031522223100_s.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/chrome-55.gif")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/giphy.gif")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_20220315_090028.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_20220513_133303.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_20220513_133355.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_20220513_133454.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_20220513_133514.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_20220513_133546.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/ms-CIJ1b2.gif")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/200.gif")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/888et9kgogn_(35).gif")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/413150_20220223191121_1.png")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/413150_20220306195146_1.png")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/20220319_164511.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2022022822191300_s.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2022022822191300_s.jpg_original")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2022031522171200_s.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2022031522194800_s.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2022031522195200_s.jpg")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2022031522195200_s.jpg_original")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2022031522195800_s.jpg")]
	public void ImageWriteTest(string fullPath)
	{
		_testOutputHelper.WriteLine(fullPath);
		string DT = new DateTime(2070, 05, 05, 02, 03, 04).ToString("yyyy:M:d H:mm:ss");
		
		Process process = new()
		{
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
	}

	[Theory]
	//TODO: test this with mp4s, mkv, mov, etc.
	public void VideoWriteTest(string fullPath)
	{
		_testOutputHelper.WriteLine(fullPath);
		string DT = new DateTime(2070, 05, 05, 02, 03, 04).ToString("yyyy:M:d H:mm:ss");
		
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