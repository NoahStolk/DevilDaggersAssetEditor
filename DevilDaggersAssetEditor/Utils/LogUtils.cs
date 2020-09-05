using log4net;
using System.Reflection;

namespace DevilDaggersAssetEditor.Utils
{
	public static class LogUtils
	{
		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	}
}