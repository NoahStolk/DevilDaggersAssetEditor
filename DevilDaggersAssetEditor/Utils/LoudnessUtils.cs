using System;
using System.Globalization;

namespace DevilDaggersAssetEditor.Utils
{
	public static class LoudnessUtils
	{
		public static bool TryReadLoudnessLine(string line, out string? assetName, out float loudness)
		{
			try
			{
				line = line
					.Replace(" ", string.Empty, StringComparison.InvariantCulture) // Remove spaces to make things easier.
					.TrimEnd('.'); // Remove dots at the end of the line. (The original loudness file has one on line 154 for some reason...)

				int equalsIndex = line.IndexOf('=', StringComparison.InvariantCulture);

				assetName = line.Substring(0, equalsIndex);
				loudness = float.Parse(line.Substring(equalsIndex + 1, line.Length - assetName.Length - 1), CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
				assetName = null;
				loudness = 0;
				return false;
			}
		}
	}
}