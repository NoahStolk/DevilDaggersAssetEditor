using System;
using System.IO;
using System.Reflection;

namespace DevilDaggersAssetEditor.Utils
{
	public static class AssemblyUtils
	{
		public static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();

		public static Stream GetContentStream(string relativeContentName)
			=> CurrentAssembly.GetManifestResourceStream($"DevilDaggersAssetEditor.Content.{relativeContentName}") ?? throw new Exception($"Could not retrieve content stream '{relativeContentName}'.");
	}
}