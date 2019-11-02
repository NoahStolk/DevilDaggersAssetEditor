using DevilDaggersCore.Tools;
using System;

namespace DevilDaggersAssetEditor.Code.Network
{
	public sealed class NetworkHandler
	{
		private VersionResult versionResult = new VersionResult(null, null, new Exception("Version has not yet been retrieved."));
		public VersionResult VersionResult
		{
			get => versionResult;
			set
			{
				versionResult = value;
				if (versionResult.Exception != null)
					App.Instance.ShowError($"Error retrieving version number for '{App.ApplicationName}'", versionResult.Exception.Message, versionResult.Exception.InnerException);
			}
		}

		private static readonly Lazy<NetworkHandler> lazy = new Lazy<NetworkHandler>(() => new NetworkHandler());
		public static NetworkHandler Instance => lazy.Value;

		private NetworkHandler()
		{
		}
	}
}