namespace Date_Taken_Extractor;

///Contains static methods for making it easy to get Date Taken metadata from photos and videos.
public static class DateTakenExtractor
{
	///Used to represent where a Date Taken value came from in a file.
	public enum DateTakenSrc
	{
		///Date Taken came from file's metadata.
		Metadata, 
		
		///Date Taken came from filename.
		Filename,
		
		///No Date Taken in metadata or filename.
		None
	}
	
	///<summary>First tries to find Date Taken in the metadata of the file. If it can, uses that. If it can't, looks in the filename. If no data in both, return null.</summary>
	/// <param name="fullPath">The full path to the file.</param>
	/// <param name="dateTaken">The DateTime? variable to store the Date Taken in.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is a file that doesn't exist.</exception>
	/// <returns>DateTakenSrc enum representing where the Date Taken came from.</returns>
	public static DateTakenSrc GetDateTimeAuto(string fullPath, out DateTime? dateTaken)
	{
		if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
		if (!File.Exists(fullPath)) throw new ArgumentException("File specified does not exist.");
	}
	
	///<summary>Date Taken metadata from just the file's internal metadata.</summary>
	///<param name="fullPath">The full path to the file.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is a file that doesn't exist.</exception>
	///<returns>The Date Taken that was found in the metadata, otherwise null.</returns>
	public static DateTime? GetDateTakenFromMetadata(string fullPath)
	{
		if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
		if (!File.Exists(fullPath)) throw new ArgumentException("File specified does not exist.");
	}

	///<summary>Get Date Taken metadata from just the filename.</summary>
	///<param name="fullPath">The full path to the file.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is a file that doesn't exist.</exception>
	///<returns>The Date Taken that was found in the filename, otherwise null.</returns>
	public static DateTime? GetDateTakenFromFilename(string fullPath)
	{
		if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
		if (!File.Exists(fullPath)) throw new ArgumentException("File specified does not exist.");
	}
	
	///<summary>Get Date Taken from metadata AND the filename.</summary>
	///<param name="fullPath">The full path to the file.</param>
	///<param name="metadataDT">The DateTime? variable to store the metadata Date Taken in.</param>
	///<param name="filenameDT">The DateTime? variable to store the filename Date Taken in.</param>
	///<exception cref="ArgumentNullException">Thrown if fullPath is null.</exception>
	///<exception cref="ArgumentException">Thrown if fullPath is a file that doesn't exist.</exception>
	public static void GetDateTakenFromBoth(string fullPath, out DateTime? metadataDT, out DateTime? filenameDT)
	{
		if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
		if (!File.Exists(fullPath)) throw new ArgumentException("File specified does not exist.");
	}

	private static DateTime? GetFilenameDT(string filename)
	{
		//TODO: try regex here lmao
	}
}