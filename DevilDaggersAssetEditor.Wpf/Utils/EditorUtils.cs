using System;
using System.Reflection;

namespace DevilDaggersAssetEditor.Wpf.Utils
{
	public static class EditorUtils
	{
		public static Uri MakeUri(string localPath)
			=> new($"pack://application:,,,/{Assembly.GetCallingAssembly().GetName().Name};component/{localPath}");

		public static string ToTimeString(int milliseconds)
		{
			TimeSpan timeSpan = new(0, 0, 0, 0, milliseconds);
			if (timeSpan.Days > 0)
				return $"{timeSpan:dd\\:hh\\:mm\\:ss\\.fff}";
			if (timeSpan.Hours > 0)
				return $"{timeSpan:hh\\:mm\\:ss\\.fff}";
			return $"{timeSpan:mm\\:ss\\.fff}";
		}

		public static System.Windows.Media.Color FromRgbTuple((byte R, byte G, byte B) tuple)
			=> System.Windows.Media.Color.FromRgb(tuple.R, tuple.G, tuple.B);
	}
}
