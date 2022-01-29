namespace DevilDaggersAssetEditor.Extensions;

public static class StringExtensions
{
	public static string TrimEnd(this string s, string value)
	{
		if (!s.EndsWith(value))
			return s;

		return s.Remove(s.LastIndexOf(value));
	}
}
