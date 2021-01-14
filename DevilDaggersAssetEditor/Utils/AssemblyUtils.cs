using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DevilDaggersAssetEditor.Utils
{
	public static class AssemblyUtils
	{
		public static Assembly CurrentAssembly => Assembly.GetExecutingAssembly();

		internal static Version LocalVersion => Version.Parse(FileVersionInfo.GetVersionInfo(CurrentAssembly.Location).FileVersion ?? throw new("Could not get file version from current assembly."));

		public static Stream GetContentStream(string relativeContentName)
			=> CurrentAssembly.GetManifestResourceStream($"DevilDaggersAssetEditor.Content.{relativeContentName}") ?? throw new($"Could not retrieve content stream '{relativeContentName}'.");
	}
}
