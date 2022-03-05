using DevilDaggersAssetEditor.Binaries.Chunks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DevilDaggersAssetEditor.Tests;

[TestClass]
public class ObjParseTests
{
	[DataTestMethod]
	[DataRow("3dsMax-claw.obj")]
	[DataRow("3dsMax-hand.obj")]
	[DataRow("Wings3d-cylinder.obj")]
	public void ParseObj(string fileName)
	{
		ModelChunk.ReadObj(Path.Combine("Resources", fileName), out _, out _, out _, out _);
	}
}
