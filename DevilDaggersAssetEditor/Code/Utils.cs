using System;
using System.Reflection;

namespace DevilDaggersAssetEditor.Code
{
	public static class Utils
	{
		public static Uri MakeUri(string localPath)
		{
			return new Uri($"pack://application:,,,/{Assembly.GetCallingAssembly().GetName().Name};component/{localPath}");
		}
	}
}