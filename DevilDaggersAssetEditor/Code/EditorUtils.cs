using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Chunks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DevilDaggersAssetEditor.Code
{
	public static class EditorUtils
	{
		public static Uri MakeUri(string localPath)
		{
			return new Uri($"pack://application:,,,/{Assembly.GetCallingAssembly().GetName().Name};component/{localPath}");
		}

		public static string ToTimeString(int milliseconds)
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, milliseconds);
			if (timeSpan.Days > 0)
				return $"{timeSpan:dd\\:hh\\:mm\\:ss\\.fff}";
			if (timeSpan.Hours > 0)
				return $"{timeSpan:hh\\:mm\\:ss\\.fff}";
			return $"{timeSpan:mm\\:ss\\.fff}";
		}

		public static void GenerateDDJsonFiles()
		{
			List<ModelBindingAsset> modelBindings = new List<ModelBindingAsset>();
			List<ShaderAsset> shaders = new List<ShaderAsset>();
			List<ModelAsset> models = new List<ModelAsset>();
			List<TextureAsset> textures = new List<TextureAsset>();

			foreach (string path in Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\Extracted\dd", "*.*", SearchOption.AllDirectories))
			{
				switch (Path.GetExtension(path))
				{
					case ".txt":
						modelBindings.Add(new ModelBindingAsset(Path.GetFileNameWithoutExtension(path), "?", "?", nameof(ModelBindingChunk)));
						break;
					case ".glsl":
						shaders.Add(new ShaderAsset(Path.GetFileNameWithoutExtension(path), "?", "?", nameof(ShaderChunk)));
						break;
					case ".obj":
						models.Add(new ModelAsset(Path.GetFileNameWithoutExtension(path), "?", "?", nameof(ModelChunk)));
						break;
					case ".png":
						Image image = Image.FromFile(path);
						textures.Add(new TextureAsset(Path.GetFileNameWithoutExtension(path), "?", "?", nameof(TextureChunk), new Point(image.Width, image.Height), ""));
						break;
				}
			}

			JsonUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Model Bindings.json", modelBindings);
			JsonUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Shaders.json", shaders);
			JsonUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Models.json", models);
			JsonUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Textures.json", textures);
		}

		public static void GenerateCoreJsonFile()
		{
			List<ShaderAsset> shaders = new List<ShaderAsset>();

			foreach (string file in Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\Extracted\core", "*.*", SearchOption.AllDirectories))
			{
				switch (Path.GetExtension(file))
				{
					case ".glsl":
						shaders.Add(new ShaderAsset(Path.GetFileNameWithoutExtension(file), "?", "?", nameof(ShaderChunk)));
						break;
				}
			}

			JsonUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\core\Shaders.json", shaders);
		}

		public static void ReadTextureDataFromTextFile()
		{
			List<TextureAsset> textures;
			using (StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.dd.Textures.json")))
				textures = JsonConvert.DeserializeObject<List<TextureAsset>>(sr.ReadToEnd());

			foreach (string line in File.ReadAllLines(@"C:\Users\NOAH\Desktop\textures.txt"))
			{
				string[] split = line.Split('\t');
				string name = split[0].Replace(".png", "");

				TextureAsset t = textures.FirstOrDefault(a => a.AssetName == name);

				t.EntityName = split[1];
				t.Description = split[2];
				t.ModelBinding = split[3];
			}

			JsonUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Textures.json", textures);
		}
	}
}