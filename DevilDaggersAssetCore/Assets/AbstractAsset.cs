using DevilDaggersAssetCore.ModFiles;
using Newtonsoft.Json;

namespace DevilDaggersAssetCore.Assets
{
	[JsonObject(MemberSerialization.OptIn)]
	public abstract class AbstractAsset
	{
		[JsonProperty]
		public string AssetName { get; }
		[JsonProperty]
		public string Description { get; }
		[JsonProperty]
		public string TypeName { get; }

		public string EditorPath { get; set; } = "<NONE SPECIFIED>";

		protected AbstractAsset(string assetName, string description, string typeName)
		{
			AssetName = assetName;
			Description = description;
			TypeName = typeName;
		}

		public virtual GenericUserAsset ToUserAsset()
		{
			return new GenericUserAsset(AssetName, EditorPath);
		}

		public virtual void ImportValuesFromUserAsset(GenericUserAsset userAsset)
		{
			EditorPath = userAsset.EditorPath;
		}
	}
}