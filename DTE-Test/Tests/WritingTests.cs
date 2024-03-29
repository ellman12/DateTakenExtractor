﻿namespace DTE_Test.Tests;

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

	[Test]
	public void WritePngDateTaken()
	{
		IEnumerable<TF> pngs = TF.TestFiles.Where(testFile => testFile.Filename is "c142964" or "Hello World");

		foreach (TF file in pngs)
		{
			string path = Path.Join(TempTestFilesPath, file.Filename);
			
			DateTime newDateTaken = DateTime.Today + new TimeSpan(10, 30, 0); //10:30 AM
			D.UpdateDateTaken(path, newDateTaken);

			DateTime? updatedDateTaken = D.GetDateTakenFromMetadata(path);
			if (updatedDateTaken != newDateTaken)
				Assert.Fail($"{file.Filename}'s new metadata DT of {updatedDateTaken} does not match what what is was supposed to be: {newDateTaken}");
		}
	}

	[Test]
	public void WriteVideoDateTaken()
	{
		IEnumerable<TF> jpgs = TF.TestFiles.Where(testFile => testFile.Filename.ToLower().EndsWith(".mp4") || testFile.Filename.ToLower().EndsWith(".mov"));

		foreach (TF file in jpgs)
		{
			string path = Path.Join(TempTestFilesPath, file.Filename);
			
			DateTime newDateTaken = DateTime.Today + new TimeSpan(10, 30, 0); //10:30 AM
			D.UpdateDateTaken(path, newDateTaken.ToLocalTime());

			DateTime? updatedDateTaken = D.GetDateTakenFromMetadata(path);
			if (updatedDateTaken != newDateTaken)
				Assert.Fail($"{file.Filename}'s new metadata DT of {updatedDateTaken} does not match what what is was supposed to be: {newDateTaken}");
		}
	}
	
	[Test]
	public void ClearMetadataDateTaken()
	{
		IEnumerable<TF> files = TF.TestFiles.Where(testFile => testFile.MetadataDT != null);

		foreach (TF file in files)
		{
			string path = Path.Join(TempTestFilesPath, file.Filename);
			D.UpdateDateTaken(path, null);
			
			DateTime? updatedDateTaken = D.GetDateTakenFromMetadata(path);
			if (updatedDateTaken != null)
				Assert.Fail($"{file.Filename}'s metadata DT should be null, but was actually {updatedDateTaken}");
		}
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		TryDeleteTempFiles();
	}
}