using DevilDaggersCore.Mods;

namespace DevilDaggersAssetEditor.ModFiles;

public class AudioUserAsset : UserAsset
{
	public AudioUserAsset(string assetName, string editorPath, float loudness)
		: base(AssetType.Audio, assetName, editorPath)
	{
		Loudness = loudness;
	}

	public float Loudness { get; }
}