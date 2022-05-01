using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace DevilDaggersAssetEditor.Wpf.Caching;

public sealed class ModScreenshotCache
{
	private readonly Dictionary<(string ModName, string ScreenshotFileName), BitmapImage> _cache = new();

	private static readonly Lazy<ModScreenshotCache> _lazy = new(() => new());

	private ModScreenshotCache()
	{
	}

	public static ModScreenshotCache Instance => _lazy.Value;

	public BitmapImage? GetScreenshot(string modName, string screenshotFileName)
	{
		(string ModName, string ScreenshotFileName) key = (modName, screenshotFileName);
		if (_cache.ContainsKey(key))
			return _cache[key];

		BitmapImage? image = DownloadScreenshot(modName, screenshotFileName);
		if (image == null)
			return null;

		_cache.Add(key, image);
		return image;
	}

	private static BitmapImage? DownloadScreenshot(string modName, string screenshotFileName)
	{
		string url = $"https://devildaggers.info/api/mod-screenshots?modName={Uri.EscapeDataString(modName)}&fileName={Uri.EscapeDataString(screenshotFileName)}";

		try
		{
			return new(new Uri(url));
		}
		catch (Exception ex)
		{
			App.Instance.ShowError("Could not download screenshot", $"Unable to download screenshot from '{url}'.", ex);
			return null;
		}
	}

	public void Clear()
		=> _cache.Clear();
}
