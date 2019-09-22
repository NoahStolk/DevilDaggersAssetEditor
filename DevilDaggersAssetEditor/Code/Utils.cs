using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Chunks;
using System.Collections.Generic;
using DevilDaggersAssetCore;

namespace DevilDaggersAssetEditor.Code
{
	public static class Utils
	{
		public static Uri MakeUri(string localPath)
		{
			return new Uri($"pack://application:,,,/{Assembly.GetCallingAssembly().GetName().Name};component/{localPath}");
		}

		public static Assembly GetAssemblyByName(string name)
		{
			return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
		}

		public static void GenerateDDJsonFiles()
		{
			List<ModelBindingAsset> modelBindings = new List<ModelBindingAsset>();
			List<ShaderAsset> shaders = new List<ShaderAsset>();
			List<ModelAsset> models = new List<ModelAsset>();
			List<TextureAsset> textures = new List<TextureAsset>();

			foreach (string file in Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\Extracted\dd", "*.*", SearchOption.AllDirectories))
			{
				switch (Path.GetExtension(file))
				{
					case ".txt":
						modelBindings.Add(new ModelBindingAsset(Path.GetFileNameWithoutExtension(file), "?", nameof(ModelBindingChunk)));
						break;
					case ".glsl":
						shaders.Add(new ShaderAsset(Path.GetFileNameWithoutExtension(file), "?", nameof(ShaderChunk)));
						break;
					case ".obj":
						models.Add(new ModelAsset(Path.GetFileNameWithoutExtension(file), "?", nameof(ModelChunk)));
						break;
					case ".png":
						textures.Add(new TextureAsset(Path.GetFileNameWithoutExtension(file), "?", nameof(TextureChunk)));
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
						shaders.Add(new ShaderAsset(Path.GetFileNameWithoutExtension(file), "?", nameof(ShaderChunk)));
						break;
				}
			}

			JsonUtils.SerializeToFile(@"C:\Users\NOAH\source\repos\DevilDaggersAssetEditor\DevilDaggersAssetCore\Content\core\Shaders.json", shaders);
		}
	}
}