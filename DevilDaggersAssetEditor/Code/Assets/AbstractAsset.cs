namespace DevilDaggersAssetEditor.Code.Assets
{
	public abstract class AbstractAsset
	{
		public string FileName { get; set; }
		public string Description { get; set; }

		protected AbstractAsset(string fileName, string description)
		{
			FileName = fileName;
			Description = description;
		}
	}
}