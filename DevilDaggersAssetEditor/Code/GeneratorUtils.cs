using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Chunks;
using JsonUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DevilDaggersAssetEditor.Code
{
	public static class GeneratorUtils
	{
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
						modelBindings.Add(new ModelBindingAsset(Path.GetFileNameWithoutExtension(path), "?", "?", nameof(ModelBindingChunk)));
						break;
					case ".glsl":
						shaders.Add(new ShaderAsset(Path.GetFileNameWithoutExtension(path), "?", "?", nameof(ShaderChunk)));
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

						models.Add(new ModelAsset(Path.GetFileNameWithoutExtension(path), "?", "?", nameof(ModelChunk), new[] { v, vt, vn }.Max(), f));
						break;
					case ".png":
						Image image = Image.FromFile(path);
						textures.Add(new TextureAsset(Path.GetFileNameWithoutExtension(path), "?", "?", nameof(TextureChunk), new Point(image.Width, image.Height), ""));
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
						shaders.Add(new ShaderAsset(Path.GetFileNameWithoutExtension(file), "?", "?", nameof(ShaderChunk)));
						break;
				}
			}

			JsonFileUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\core\Shaders.json", shaders, false);
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

			JsonFileUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\dd\Textures.json", textures, false);
		}
	}
}