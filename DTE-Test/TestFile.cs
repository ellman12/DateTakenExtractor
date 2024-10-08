using System.Text.Json;

namespace DTE_Test;

public record TestFile(string Filename, DateTime? MetadataDT, DateTime? FilenameDT)
{
	public static readonly List<TestFile> TestFiles;

	public static string TestFilesPath => Path.Join(Environment.CurrentDirectory.Replace("\\", "/").Replace("bin/Debug/net7.0", null), "TestFiles");
	
	public static string JsonPath => Path.Join(TestFilesPath, "TestFiles.json");

	static TestFile()
	{
		string json = File.ReadAllText(JsonPath);
		TestFiles = JsonSerializer.Deserialize<List<TestFile>>(json)!;

		foreach (TF tf in TestFiles)
		{
			string path = Path.Join(TestFilesPath, tf.Filename);
			if (!File.Exists(path))
				throw new FileNotFoundException($"{tf.Filename} does not exist");
		}
	}

	///(Re-)Initializes the JSON file containing data about each file. Only needs to be ran when modifying the 'Test Files' folder.
	private static void JsonInitialization()
	{
		List<TestFile> items = new();
	
		foreach (string file in Directory.GetFiles(TestFilesPath))
		{
			if (file.EndsWith(".json")) continue;
			
			string filename = Path.GetFileName(file);
			TestFile newItem = new(filename, D.GetDateTakenFromMetadata(file), D.GetDateTakenFromFilename(filename));
			items.Add(newItem);
		}
	
		File.WriteAllText(JsonPath, JsonSerializer.Serialize(items));
	}
}