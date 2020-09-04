﻿namespace DevilDaggersAssetEditor
{
	public static class LoudnessUtils
	{
		public static bool TryReadLoudnessLine(string line, out string assetName, out float loudness)
		{
			try
			{
				assetName = line.Substring(0, line.IndexOf('=')).Trim();
				loudness = float.Parse(line.Substring(line.IndexOf('=') + 1, line.Length - assetName.Length - 1));
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