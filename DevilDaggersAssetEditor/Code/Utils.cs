using System;
using System.Reflection;

namespace DevilDaggersAssetEditor.Code
{
	public static class Utils
	{
		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public static Uri MakeUri(string localPath)
		{
			return new Uri($"pack://application:,,,/{Assembly.GetName().Name};component/{localPath}");
		}
	}
}