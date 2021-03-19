using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.Json;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersCore.Extensions;
using DevilDaggersCore.Wpf.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevilDaggersAssetEditor.Wpf.ModFiles
{
	public sealed class ModFileHandler
	{
		private bool _hasUnsavedChanges;

		private static readonly Lazy<ModFileHandler> _lazy = new(() => new());

		private ModFileHandler()
		{
		}

		public static ModFileHandler Instance => _lazy.Value;

		public List<UserAsset> ModFile { get; set; } = new();

		public bool HasUnsavedChanges
		{
			get => _hasUnsavedChanges;
			set
			{
				_hasUnsavedChanges = value;
				App.Instance.UpdateMainWindowTitle();
			}
		}

		public string ModFileName { get; private set; } = "(new mod)";
		public string ModFileLocation { get; private set; } = string.Empty;

		public void UpdateModFileState(string name, string fileLocation)
		{
			HasUnsavedChanges = false;

			ModFileName = name;
			ModFileLocation = fileLocation;

			App.Instance.UpdateMainWindowTitle();
		}

		/// <summary>
		/// Asks the user to save the file before proceeding.
		/// </summary>
		/// <returns><see langword="true"/> if the following action should be cancelled.</returns>
		public bool ProceedWithUnsavedChanges()
		{
			if (!HasUnsavedChanges)
				return false;

			ConfirmWindow confirmWindow = new("Save changes?", "The current mod has unsaved changes. Save before proceeding?", false);
			confirmWindow.ShowDialog();

			if (confirmWindow.IsConfirmed == true)
				FileSave();

			return confirmWindow.IsConfirmed == null;
		}

		private void SaveAssets()
		{
			List<AbstractAsset> assets = App.Instance.MainWindow!.AssetTabControls.SelectMany(atc => atc.GetAssets()).ToList();

			ModFile.Clear();
			foreach (AbstractAsset asset in assets)
				ModFile.Add(asset.ToUserAsset());
		}

		public void FileSave()
		{
			SaveAssets();

			if (File.Exists(ModFileLocation))
			{
				JsonFileUtils.SerializeToFile(ModFileLocation, ModFile, true);
				HasUnsavedChanges = false;
			}
			else
			{
				FileSaveAs();
			}
		}

		public void FileSaveAs()
		{
			SaveAssets();

			SaveFileDialog dialog = new() { Filter = GuiUtils.ModFileFilter };
			dialog.OpenModsRootFolder();

			bool? result = dialog.ShowDialog();
			if (result == true)
			{
				JsonFileUtils.SerializeToFile(ModFileLocation, ModFile, true);
				UpdateModFileState(Path.GetFileName(dialog.FileName), dialog.FileName);
			}
		}

		public static List<UserAsset> GetAssetsFromModFilePath(string path)
		{
			List<UserAsset>? assets = JsonFileUtils.TryDeserializeFromFile<List<UserAsset>>(path, true);
			if (assets == null)
				return new();

			UserHandler.Instance.Cache.OpenedModFilePath = path;

			return assets;
		}

		/// <summary>
		/// Creates a mod file based on the assets found in the directory specified by <paramref name="path"/> and places it inside that directory.
		/// </summary>
		public static void CreateModFileFromPath(string path)
		{
			List<UserAsset> assets = GetAssets(path);

			string folderName = new DirectoryInfo(path).Name;
			JsonFileUtils.SerializeToFile(Path.Combine(path, $"{folderName}.ddae"), assets, true);
		}

		private static List<UserAsset> GetAssets(string directory)
		{
			Dictionary<string, float> loudnessValues = new();
			string? loudnessFilePath = Array.Find(Directory.GetFiles(directory, "*.ini", SearchOption.AllDirectories), p => Path.GetFileNameWithoutExtension(p) == "loudness");
			if (loudnessFilePath != null)
			{
				foreach (string line in File.ReadAllLines(loudnessFilePath))
				{
					if (LoudnessUtils.TryReadLoudnessLine(line, out string? assetName, out float loudness))
						loudnessValues.Add(assetName!, loudness);
				}
			}

			List<UserAsset> assets = new();

			foreach (string path in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
			{
				string name = Path.GetFileNameWithoutExtension(path);
				AssetType? assetType = Path.GetExtension(path).GetAssetType();
				if (!assetType.HasValue)
					continue;

				if (assetType == AssetType.Shader)
				{
					if (!assets.Any(a => a.AssetType == AssetType.Shader && a.AssetName == name))
					{
						string normalizedPath = path.TrimEnd("_vertex.glsl").TrimEnd("_fragment.glsl");

						assets.Add(new ShaderUserAsset(
							name.TrimEnd("_vertex").TrimEnd("_fragment"),
							$"{normalizedPath}_vertex.glsl",
							$"{normalizedPath}_fragment.glsl"));
					}
				}
				else if (assetType == AssetType.Audio)
				{
					float loudness = 1;
					if (loudnessValues.ContainsKey(name))
						loudness = loudnessValues[name];
					assets.Add(new AudioUserAsset(name, path, loudness));
				}
				else
				{
					assets.Add(new UserAsset(assetType.Value, name, path));
				}
			}

			return assets;
		}
	}
}
