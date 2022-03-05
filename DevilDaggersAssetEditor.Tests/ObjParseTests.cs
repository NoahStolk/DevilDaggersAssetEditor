using DevilDaggersAssetEditor.Binaries.Chunks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DevilDaggersAssetEditor.Tests;

[TestClass]
public class ObjParseTests
{
	[DataTestMethod]
	[DataRow("3dsMax-claw.obj")]
	[DataRow("3dsMax-hand.obj")]
	[DataRow("Wings3d-cylinder.obj")]
	[DataRow("Wings3d-cube.obj")]
	public void ParseObj(string fileName)
	{
		ModelChunk.ReadObj(Path.Combine("Resources", fileName), out _, out _, out _, out _);
	}

	[DataTestMethod]
	[DataRow("Wings3d-cube-invalid-face.obj")]
	[DataRow("Wings3d-cube-no-uv.obj")]
	public void ParseInvalidObj(string fileName)
	{
		Assert.ThrowsException<Exception>(() => ModelChunk.ReadObj(Path.Combine("Resources", fileName), out _, out _, out _, out _));
	}
}
