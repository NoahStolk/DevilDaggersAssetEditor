namespace DevilDaggersAssetCore.Assets
{
	public abstract class AbstractAsset
	{
		public string AssetName { get; set; }
		public string Description { get; set; }

		public string EditorPath { get; set; }

		protected AbstractAsset(string fileName, string description)
		{
			AssetName = fileName;
			Description = description;
		}
	}
}