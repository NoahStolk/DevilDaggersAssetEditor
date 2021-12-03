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

	public BitmapImage GetScreenshot(string modName, string screenshotFileName)
	{
		(string ModName, string ScreenshotFileName) key = (modName, screenshotFileName);
		if (_cache.ContainsKey(key))
			return _cache[key];

		BitmapImage image = new(new Uri($"https://devildaggers.info/mod-screenshots/{modName}/{screenshotFileName}"));
		_cache.Add(key, image);
		return image;
	}

	public void Clear()
		=> _cache.Clear();
}