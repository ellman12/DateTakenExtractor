namespace DTE_Test.Tests;

[TestFixture]
public class WritingTests
{
	private static string TempTestFilesPath => Path.Join(Environment.CurrentDirectory.Replace("\\", "/").Replace("bin/Debug/net7.0", null), "TempTestFiles");

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		TryDeleteTempFiles();
		CopyDirectory(TF.TestFilesPath, TempTestFilesPath);
	}

	private static void CopyDirectory(string srcDir, string destDir)
	{
		if (!Directory.Exists(destDir))
			Directory.CreateDirectory(destDir);

		string[] files = Directory.GetFiles(srcDir, "*", SearchOption.AllDirectories);
		foreach (string file in files)
		{
			string destinationFile = Path.Combine(destDir, Path.GetFileName(file));
			File.Copy(file, destinationFile);
		}
	}

	private static void TryDeleteTempFiles()
	{
		if (Directory.Exists(TempTestFilesPath))
			Directory.Delete(TempTestFilesPath, true);
	}

	[Test]
	public void WriteJpgDateTaken()
	{
		IEnumerable<TF> jpgs = TF.TestFiles.Where(testFile => testFile.Filename.ToLower().EndsWith(".jpg"));

		foreach (TF file in jpgs)
		{
			string path = Path.Join(TempTestFilesPath, file.Filename);
			
			DateTime newDateTaken = DateTime.Today + new TimeSpan(10, 30, 0); //10:30 AM
			D.UpdateDateTaken(path, newDateTaken);

			DateTime? updatedDateTaken = D.GetDateTakenFromMetadata(path);
			if (updatedDateTaken != newDateTaken)
				Assert.Fail($"{file.Filename}'s new metadata DT of {updatedDateTaken} does not match what what is was supposed to be: {newDateTaken}");
		}
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		TryDeleteTempFiles();
	}
}