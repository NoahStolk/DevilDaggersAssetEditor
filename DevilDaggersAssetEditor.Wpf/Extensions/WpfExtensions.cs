using DevilDaggersAssetEditor.Chunks;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.Extensions
{
	public static class WpfExtensions
	{
		public static Color GetColor(this ChunkInfo chunkInfo)
			=> Color.FromRgb(chunkInfo.ColorR, chunkInfo.ColorG, chunkInfo.ColorB);
	}
}