using System.IO;
using System.Reflection;

namespace DevilDaggersAssetEditor.Utils;

public static class AssemblyUtils
{
	public static Stream GetContentStream(string relativeContentName)
		=> Assembly.GetExecutingAssembly().GetManifestResourceStream($"DevilDaggersAssetEditor.Content.{relativeContentName}") ?? throw new($"Could not retrieve content stream '{relativeContentName}'.");
}
