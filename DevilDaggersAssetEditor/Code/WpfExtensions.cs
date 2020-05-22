using DevilDaggersAssetCore.Info;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Code
{
	public static class WpfExtensions
	{
		public static Color GetColor(this ChunkInfo chunkInfo) => Color.FromRgb(chunkInfo.ColorR, chunkInfo.ColorG, chunkInfo.ColorB);
	}
}