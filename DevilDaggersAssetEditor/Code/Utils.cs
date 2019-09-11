using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DevilDaggersAssetEditor.Code
{
	public static class Utils
	{
		public static readonly string DDFolder = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";

		public static Uri MakeUri(string localPath)
		{
			return new Uri($"pack://application:,,,/{Assembly.GetCallingAssembly().GetName().Name};component/{localPath}");
		}
		
		public static Assembly GetAssemblyByName(string name)
		{
			return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
		}

		public static bool IsPathValid(string path)
		{
			try
			{
				return Path.IsPathRooted(path);
			}
			catch
			{
				return false;
			}
		}
	}
}