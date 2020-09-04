using log4net;
using System.Reflection;

namespace DevilDaggersAssetEditor
{
	public static class Logging
	{
		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	}
}