namespace DevilDaggersAssetEditor.Wpf.Utils;

public static class EditorUtils
{
	public static System.Windows.Media.Color FromRgbTuple((byte R, byte G, byte B) tuple)
		=> System.Windows.Media.Color.FromRgb(tuple.R, tuple.G, tuple.B);
}
