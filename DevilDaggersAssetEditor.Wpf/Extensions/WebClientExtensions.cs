using DevilDaggersAssetEditor.Progress;
using DevilDaggersAssetEditor.Wpf.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DevilDaggersAssetEditor.Wpf.Extensions;

public static class WebClientExtensions
{
	public static async Task<byte[]?> DownloadByteArrayAsync(this WebClient wc, string url, ProgressWrapper progress, CancellationTokenSource cancellationTokenSource)
	{
		int receivedBytes = 0;
		List<byte> content = new();

		using (Stream stream = await wc.OpenReadTaskAsync(url))
		{
			_ = int.TryParse(wc.ResponseHeaders?[HttpResponseHeader.ContentLength], out int totalBytes);
			if (totalBytes == 0)
				return Array.Empty<byte>();

			byte[] buffer = new byte[4096];

			while (true)
			{
				if (cancellationTokenSource.IsCancellationRequested)
					return null;

				int length = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
				if (length == 0)
				{
					await Task.Yield();
					break;
				}

				receivedBytes += length;
				content.AddRange(buffer[0..length]);

				progress.Report($"{receivedBytes / (float)totalBytes:0%} ({FormatUtils.FormatFileSize(receivedBytes)} / {FormatUtils.FormatFileSize(totalBytes)})", receivedBytes / (float)totalBytes);
			}
		}

		return content.ToArray();
	}
}
