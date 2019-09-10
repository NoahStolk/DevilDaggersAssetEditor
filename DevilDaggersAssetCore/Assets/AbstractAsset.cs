namespace DevilDaggersAssetCore.Assets
{
	public abstract class AbstractAsset
	{
		public string AssetName { get; }
		public string Description { get; }
		public string TypeName { get; }

		public string EditorPath { get; set; }

		protected AbstractAsset(string fileName, string description, string typeName)
		{
			AssetName = fileName;
			Description = description;
			TypeName = typeName;
		}
	}
}