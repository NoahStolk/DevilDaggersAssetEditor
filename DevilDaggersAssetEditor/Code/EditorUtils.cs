using System;
using System.Reflection;

namespace DevilDaggersAssetEditor.Code
{
	public static class EditorUtils
	{
		public static readonly int TagsMaxLength = 30;
		public static readonly int DescriptionMaxLength = 50;
		public static readonly int EditorPathMaxLength = 50;

		public static Uri MakeUri(string localPath) => new Uri($"pack://application:,,,/{Assembly.GetCallingAssembly().GetName().Name};component/{localPath}");

		public static string ToTimeString(int milliseconds)
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, milliseconds);
			if (timeSpan.Days > 0)
				return $"{timeSpan:dd\\:hh\\:mm\\:ss\\.fff}";
			if (timeSpan.Hours > 0)
				return $"{timeSpan:hh\\:mm\\:ss\\.fff}";
			return $"{timeSpan:mm\\:ss\\.fff}";
		}
	}
}