using System;
using System.Reflection;

namespace DevilDaggersAssetEditor.Wpf.Code
{
	public static class EditorUtils
	{
		public static int TagsMaxLength => (int)(App.Instance.MainWindow.CurrentTabControlSize.X / 40);
		public static int DescriptionMaxLength => (int)(App.Instance.MainWindow.CurrentTabControlSize.X / 27);
		public static int EditorPathMaxLength => (int)(App.Instance.MainWindow.CurrentTabControlSize.X / 27); // TODO: Can be larger for assets other than audio (due to loudness TextBox taking space).

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