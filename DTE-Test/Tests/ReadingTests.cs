namespace DTE_Test.Tests;

[TestFixture]
public class ReadingTests
{
	[Test]
	public void ReadBothDTTypesForAllFiles()
	{
		foreach (TF tf in TF.TestFiles)
		{
			string path = Path.Join(TF.TestFilesPath, tf.Filename);
			D.GetDateTakenFromBoth(path, out DateTime? metadataDT, out DateTime? filenameDT);
			
			if (tf.MetadataDT != metadataDT)
				Assert.Fail($"{tf.Filename}'s metadata DT of {tf.MetadataDT} does not match the expected value of {metadataDT}");
			else if (tf.FilenameDT != filenameDT)
				Assert.Fail($"{tf.Filename}'s filename DT of {tf.FilenameDT} does not match the expected value of {filenameDT}");
		}
	}
}