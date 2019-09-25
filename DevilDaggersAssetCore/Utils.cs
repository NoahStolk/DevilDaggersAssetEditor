using System;
using System.Linq;
using System.Reflection;

namespace DevilDaggersAssetCore
{
	public static class Utils
	{
		public static Assembly GetAssemblyByName(string name)
		{
			return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
		}
	}
}