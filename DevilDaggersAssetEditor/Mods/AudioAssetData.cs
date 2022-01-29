namespace DevilDaggersAssetEditor.Mods;

public class AudioAssetData : AssetData
{
	public AudioAssetData(string assetName, bool isProhibited, float defaultLoudness, bool presentInDefaultLoudness)
		: base(assetName, isProhibited)
	{
		DefaultLoudness = defaultLoudness;
		PresentInDefaultLoudness = presentInDefaultLoudness;
	}

	public float DefaultLoudness { get; }
	public bool PresentInDefaultLoudness { get; }
}
