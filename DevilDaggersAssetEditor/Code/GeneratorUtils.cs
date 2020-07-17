using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Chunks;
using DevilDaggersAssetCore.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DevilDaggersAssetEditor.Code
{
	public static class GeneratorUtils
	{
		public static void FixAudioJsonFiles()
		{
			List<AudioAsset> audioNow = JsonConvert.DeserializeObject<List<AudioAsset>>(File.ReadAllText(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\audio\Audio.json"));
			List<AudioAsset> audioThen = JsonConvert.DeserializeObject<List<AudioAsset>>(File.ReadAllText(@"C:\Users\NOAH\Desktop\lostaudio.txt"));

			foreach (AudioAsset assetThen in audioThen)
			{
				AudioAsset assetNow = audioNow.FirstOrDefault(a => a.AssetName == assetThen.AssetName);
				if (assetNow == null)
					audioNow.Add(new AudioAsset(assetThen.AssetName, assetThen.Description, new[] { "Unused" }, "AudioChunk", assetThen.Loudness, assetThen.PresentInDefaultLoudness));
			}

			File.WriteAllText(@"C:\Users\NOAH\Desktop\recovaudio.txt", JsonConvert.SerializeObject(audioNow));
		}

		public static void GenerateDdJsonFiles()
		{
			List<ModelBindingAsset> modelBindings = new List<ModelBindingAsset>();
			List<ShaderAsset> shaders = new List<ShaderAsset>();
			List<ModelAsset> models = new List<ModelAsset>();
			List<TextureAsset> textures = new List<TextureAsset>();

			foreach (string path in Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\Mods\Original\dd", "*.*", SearchOption.AllDirectories))
			{
				switch (Path.GetExtension(path))
				{
					case ".txt":
						modelBindings.Add(new ModelBindingAsset(Path.GetFileNameWithoutExtension(path), "?", new[] { "?" }, nameof(ModelBindingChunk)));
						break;
					case ".glsl":
						shaders.Add(new ShaderAsset(Path.GetFileNameWithoutExtension(path), "?", new[] { "?" }, nameof(ShaderChunk)));
						break;
					case ".obj":
						string[] lines = File.ReadAllLines(path);
						int v = 0;
						int vt = 0;
						int vn = 0;
						int f = 0;
						foreach (string line in lines)
						{
							switch (line.Split(' ')[0])
							{
								case "v": v++; break;
								case "vt": vt++; break;
								case "vn": vn++; break;
								case "f": f++; break;
							}
						}

						models.Add(new ModelAsset(Path.GetFileNameWithoutExtension(path), "?", new[] { "?" }, nameof(ModelChunk), new[] { v, vt, vn }.Max(), f));
						break;
					case ".png":
						Image image = Image.FromFile(path);
						textures.Add(new TextureAsset(Path.GetFileNameWithoutExtension(path), "?", new[] { "?" }, nameof(TextureChunk), new Point(image.Width, image.Height), "", false /*We don't know this here.*/));
						break;
				}
			}

			JsonFileUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Model Bindings.json", modelBindings, false);
			JsonFileUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Shaders.json", shaders, false);
			JsonFileUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Models.json", models, false);
			JsonFileUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Textures.json", textures, false);
		}

		public static void GenerateCoreJsonFile()
		{
			List<ShaderAsset> shaders = new List<ShaderAsset>();

			foreach (string file in Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\Extracted\core", "*.*", SearchOption.AllDirectories))
			{
				switch (Path.GetExtension(file))
				{
					case ".glsl":
						shaders.Add(new ShaderAsset(Path.GetFileNameWithoutExtension(file), "?", new[] { "?" }, nameof(ShaderChunk)));
						break;
				}
			}

			JsonFileUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\core\Shaders.json", shaders, false);
		}

		public static void ReadTextureDataFromTextFile()
		{
			List<TextureAsset> textures = AssetHandler.Instance.DdTexturesAssets;

			foreach (string line in File.ReadAllLines(@"C:\Users\NOAH\Desktop\textures.txt"))
			{
				string[] split = line.Split('\t');
				string name = split[0].Replace(".png", "");

				TextureAsset t = textures.FirstOrDefault(a => a.AssetName == name);

				//t.EntityName = split[1];
				//t.Description = split[2];
				t.ModelBinding = split[3];
			}

			JsonFileUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Textures.json", textures, false);
		}
	}
}