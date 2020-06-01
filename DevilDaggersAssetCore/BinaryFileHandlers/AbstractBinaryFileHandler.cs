using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.ModFiles;
using DevilDaggersAssetCore.User;
using JsonUtils;
using NetBase.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevilDaggersAssetCore.BinaryFileHandlers
{
	public abstract class AbstractBinaryFileHandler
	{
		protected UserSettings settings => UserHandler.Instance.settings;

		public BinaryFileType BinaryFileType { get; }

		protected AbstractBinaryFileHandler(BinaryFileType binaryFileType)
		{
			BinaryFileType = binaryFileType;
		}

		public abstract void MakeBinary(List<AbstractAsset> allAssets, string outputPath, Progress<float> progress, Progress<string> progressDescription);

		public abstract void ExtractBinary(string inputPath, string outputPath, BinaryFileType binaryFileType, Progress<float> progress, Progress<string> progressDescription);

		public abstract void ValidateFile(byte[] sourceFileBytes);

		protected void CreateModFile(string outputPath, BinaryFileType binaryFileType)
		{
			List<AbstractUserAsset> assets = binaryFileType switch
			{
				BinaryFileType.Audio => GetAudioAssets(outputPath),
				BinaryFileType.Core => GetNonAudioAssets(outputPath),
				BinaryFileType.Dd => GetNonAudioAssets(outputPath),
				BinaryFileType.Particle => GetNonAudioAssets(outputPath),
				_ => throw new NotImplementedException($"{nameof(BinaryFileType)} '{binaryFileType}' has not been implemented in the {nameof(CreateModFile)} method.")
			};
			ModFile modFile = new ModFile(Utils.GuiVersion, false, assets);

			string folderName = new DirectoryInfo(outputPath).Name;
			JsonFileUtils.SerializeToFile(Path.Combine(outputPath, $"{folderName}.{binaryFileType.ToString().ToLower()}"), modFile, true);
		}

		private List<AbstractUserAsset> GetAudioAssets(string outputPath)
		{
			string loudnessFilePath = Directory.GetFiles(outputPath, "*.ini", SearchOption.AllDirectories).FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == "loudness");
			if (loudnessFilePath == null)
				throw new Exception("Loudness file not found when attempting to create a mod based on newly extracted assets.");

			Dictionary<string, float> loudnessValues = new Dictionary<string, float>();
			foreach (string line in File.ReadAllLines(loudnessFilePath))
			{
				if (LoudnessUtils.TryReadLoudnessLine(line, out string assetName, out float loudness))
					loudnessValues.Add(assetName, loudness);
			}

			List<AbstractUserAsset> assets = new List<AbstractUserAsset>();
			foreach (string path in Directory.GetFiles(outputPath, "*.*", SearchOption.AllDirectories))
			{
				string name = Path.GetFileNameWithoutExtension(path);
				float loudness = 1;
				if (loudnessValues.ContainsKey(name))
					loudness = loudnessValues[name];
				assets.Add(new AudioUserAsset(name, path, loudness));
			}
			return assets.Cast<AbstractUserAsset>().ToList();
		}

		private List<AbstractUserAsset> GetNonAudioAssets(string outputPath)
		{
			Dictionary<string, Type> typeConversions = new Dictionary<string, Type>
			{
				{ "Model Bindings", typeof(ModelBindingUserAsset) },
				{ "Models", typeof(ModelUserAsset) },
				{ "Shaders", typeof(ShaderUserAsset) },
				{ "Textures", typeof(TextureUserAsset) },
				{ "Particles", typeof(ParticleUserAsset) }
			};

			List<AbstractUserAsset> assets = new List<AbstractUserAsset>();

			foreach (string path in Directory.GetFiles(outputPath, "*.*", SearchOption.AllDirectories))
			{
				string name = Path.GetFileNameWithoutExtension(path);

				string folderName = new DirectoryInfo(Path.GetDirectoryName(path)).Name;
				if (typeConversions.TryGetValue(folderName, out Type type))
				{
					if (type == typeof(ShaderUserAsset))
					{
						if (name.EndsWith("_vertex")) // Skip _fragment to avoid getting duplicate assets.
							assets.Add(Activator.CreateInstance(type, name.TrimEnd("_vertex"), path.TrimEnd("_vertex")) as AbstractUserAsset);
					}
					else
					{
						assets.Add(Activator.CreateInstance(type, name, path) as AbstractUserAsset);
					}
				}
			}
			return assets;
		}
	}
}