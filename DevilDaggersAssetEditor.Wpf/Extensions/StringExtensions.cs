namespace DevilDaggersAssetEditor.Wpf.Extensions
{
	public static class StringExtensions
	{
		public static string TrimLeft(this string text, int maxLength)
		{
			if (text.Length <= maxLength)
				return text;

			return text[^maxLength..].Insert(0, "...");
		}

		public static string TrimRight(this string text, int maxLength)
		{
			if (text.Length <= maxLength)
				return text;

			return $"{text.Substring(0, maxLength)}...";
		}
	}
}