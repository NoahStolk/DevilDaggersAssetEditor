using System;
using System.Diagnostics;

namespace DevilDaggersAssetEditor.Code
{
	public static class ApplicationUtils
	{
		public const string ApplicationName = "DevilDaggersAssetEditor";
		public const string ApplicationDisplayName = "Devil Daggers Asset Editor";

		public static string ApplicationDisplayNameWithVersion = $"{ApplicationDisplayName} {ApplicationVersionNumber}";

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