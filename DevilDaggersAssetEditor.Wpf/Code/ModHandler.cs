using DevilDaggersAssetEditor.Json;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;

namespace DevilDaggersAssetEditor.Wpf.Code
{
	public sealed class ModHandler
	{
		private static readonly Lazy<ModHandler> _lazy = new Lazy<ModHandler>(() => new ModHandler());

		private ModHandler()
		{
		}

		public static ModHandler Instance => _lazy.Value;

		public ModFile? GetModFileFromPath(string path, BinaryFileType binaryFileType)
		{
			// When DdaeVersion is not a string, it means it was created using an older version of DDAE that still used .NET Framework.
			// We need to remove this property because it will cause deserialization errors in .NET Core. This appears to be a breaking change between .NET Framework and .NET Core.
			// We do not care about having the mod file version here, so simply removing the property when importing a mod file is enough.
			string modJson = File.ReadAllText(path);

			// Remove any obsolete namespaces.
			modJson = modJson.Replace("DevilDaggersAssetCore", "DevilDaggersAssetEditor", StringComparison.InvariantCulture);

			// Fix DdaeVersion.
			JObject? modJsonObject = JsonConvert.DeserializeObject<JObject>(modJson);
			modJsonObject?.Property("DdaeVersion", StringComparison.InvariantCulture)?.Remove();
			File.WriteAllText(path, JsonConvert.SerializeObject(modJsonObject));

			ModFile? modFile = JsonFileUtils.DeserializeFromFile<ModFile>(path, true);

			if (modFile == null)
			{
				App.Instance.ShowMessage("Mod not loaded", "Could not parse mod file.");
				return null;
			}

			if (modFile.HasRelativePaths)
			{
				App.Instance.ShowMessage("Specify base path", "This mod file uses relative paths. Please specify a base path.");
				VistaFolderBrowserDialog basePathDialog = new VistaFolderBrowserDialog();

				if (UserHandler.Instance.Settings.EnableAssetsRootFolder && Directory.Exists(UserHandler.Instance.Settings.AssetsRootFolder))
					basePathDialog.SelectedPath = UserHandler.Instance.Settings.AssetsRootFolder;

				if (basePathDialog.ShowDialog() == true)
				{
					foreach (AbstractUserAsset asset in modFile.Assets)
						asset.EditorPath = Path.Combine(basePathDialog.SelectedPath, asset.EditorPath);
				}
			}

			switch (binaryFileType)
			{
				case BinaryFileType.Audio: UserHandler.Instance.Cache.OpenedAudioModFilePath = path; break;
				case BinaryFileType.Core: UserHandler.Instance.Cache.OpenedCoreModFilePath = path; break;
				case BinaryFileType.Dd: UserHandler.Instance.Cache.OpenedDdModFilePath = path; break;
				case BinaryFileType.Particle: UserHandler.Instance.Cache.OpenedParticleModFilePath = path; break;
				default: throw new NotImplementedException($"{nameof(BinaryFileType)} {binaryFileType} not implemented in {nameof(GetModFileFromPath)} method.");
			}

			return modFile;
		}
	}
}