namespace DevilDaggersAssetEditor.Wpf.Utils;

public static class FormatUtils
{
	public static string FormatFileSize(long value) => value switch
	{
		> 1_000_000_000_000 => $"{value / 1_000_000_000_000f:0.0} TB",
		> 1_000_000_000 => $"{value / 1_000_000_000f:0.0} GB",
		> 1_000_000 => $"{value / 1_000_000f:0.0} MB",
		> 1_000 => $"{value / 1_000f:0.0} KB",
		_ => $"{value} bytes",
	};
}