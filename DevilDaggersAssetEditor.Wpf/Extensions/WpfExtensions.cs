using DevilDaggersAssetEditor.Info;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.Extensions
{
	public static class WpfExtensions
	{
		public static Color GetColor(this ChunkInfo chunkInfo) => Color.FromRgb(chunkInfo.ColorR, chunkInfo.ColorG, chunkInfo.ColorB);

		public static string TrimLeft(this string text, int maxLength)
		{
			if (text.Length <= maxLength)
				return text;

			return text.Substring(text.Length - maxLength).Insert(0, "...");
		}

		public static string TrimRight(this string text, int maxLength)
		{
			if (text.Length <= maxLength)
				return text;

			return $"{text.Substring(0, maxLength)}...";
		}
	}
}