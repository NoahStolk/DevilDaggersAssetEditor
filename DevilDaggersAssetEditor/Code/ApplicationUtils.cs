using System;
using System.Diagnostics;

namespace DevilDaggersAssetEditor.Code
{
	public static class ApplicationUtils
	{
		public static string ApplicationDisplayName = $"Devil Daggers Asset Editor {ApplicationVersionNumber}";
		public const string ApplicationName = "DevilDaggersAssetEditor";

		private static Version applicationVersionNumber;
		public static Version ApplicationVersionNumber
		{
			get
			{
				if (applicationVersionNumber == null)
					applicationVersionNumber = Version.Parse(FileVersionInfo.GetVersionInfo(App.Instance.Assembly.Location).FileVersion);
				return applicationVersionNumber;
			}
		}
	}
}