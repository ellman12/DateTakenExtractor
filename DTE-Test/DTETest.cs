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
		// string DT = new DateTime(2070, 05, 05, 02, 03, 04).ToString("yyyy:M:d H:mm:ss");
		string? DT = null;

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
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/VID_20211121_161132.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/VID_20211230_092341.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/VID_20211230_105702.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/VID_20220201_171817.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/VID_20220205_185148.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/video(1).mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/video.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/WHO_TF_ASKED.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021020521531600_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021071319494200_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021071323064500_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021080122494500_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021080122562700_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021080514474000_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021080613113400_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021080613311500_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021080621124000_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021082620141400_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021082919492400_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021091116042200_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021091116052200_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021122223563300_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2021122321220000_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2022010222005500_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/2022010421142700_s.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/VID_20220107_072711.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/20210829_190606.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/20211027_210655.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/VID_20210714_194614.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/VID_20210714_194716.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/VID_20210829_094110.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/VID_20211016_113455.mp4")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_2241.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_2286.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_2455.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_2658.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_2659.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_2660.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_2671.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_2983.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_3246.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_3267.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_4418.mov")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_5597.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_5601.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/TWUC6365.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/video0-23-1.mov")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_0039.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_2296.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_1724.mov")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/DSC_6663.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/720p.mov")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/273205876_893087381357669_4286973116829744367_n.mov")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_0067.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_0841.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_1027.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_1274.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_1352.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_1368.MOV")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_1723.mov")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_1726.mov")]
	[InlineData("C:/Users/Elliott/Downloads/Photos-001/IMG_2048.MOV")]
	public void VideoWriteTest(string fullPath)
	{
		_testOutputHelper.WriteLine(fullPath);
		//Videos require DateTime to be in UTC for some reason: https://exiftool.org/forum/index.php?PHPSESSID=a68f2cbabc087b534d7ac88e55fb932d&topic=11880.msg64084#msg64084
		// string DT = new DateTime(2019, 5, 1, 20, 20, 0).ToUniversalTime().ToString("yyyy:M:d H:mm:ss");
		string? DT = null;
		
		using Process process = new()
		{
			StartInfo = new ProcessStartInfo
			{
				//https://exiftool.org/forum/index.php?topic=11100.msg59329#msg59329
				//https://exiftool.org/forum/index.php?topic=11272
				FileName = "exiftool",
				Arguments = $"\"{fullPath}\" -overwrite_original -CreateDate=\"{DT}\" -ModifyDate=\"{DT}\" -Quicktime:DateTimeOriginal=\"{DT}\"",
				CreateNoWindow = true,
				RedirectStandardError = false,
				RedirectStandardInput = false,
				RedirectStandardOutput = false,
				WindowStyle = ProcessWindowStyle.Hidden
			}
		};
		process.Start();
		process.WaitForExit();
	}
}