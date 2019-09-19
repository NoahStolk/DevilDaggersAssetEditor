namespace DevilDaggersAssetEditor.Code.Web
{
	public static class UrlUtils
	{
		private static readonly string baseUrl = "https://devildaggers.info";

		public static string DiscordInviteLink => "https://discord.gg/NF32j8S";

		public static string GetToolVersions => $"{baseUrl}/API/GetToolVersions";

		public static string SourceCode => "https://bitbucket.org/NoahStolk/devildaggersasseteditor/src/master/";

		public static string ApplicationDownloadUrl(string versionNumber) => $"{baseUrl}/tools/{ApplicationUtils.ApplicationName}/{ApplicationUtils.ApplicationName}{versionNumber}.zip";
	}
}