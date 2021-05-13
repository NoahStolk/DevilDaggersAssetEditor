using DevilDaggersAssetEditor.Progress;
using DevilDaggersAssetEditor.Wpf.Extensions;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class DownloadAndInstallModWindow : Window
	{
		private readonly CancellationTokenSource _cancellationTokenSource = new();

		public DownloadAndInstallModWindow()
		{
			InitializeComponent();
		}

		public async Task DownloadAndInstall(string modsDirectory, string modName)
		{
			TextBlockModName.Text = $"Downloading and installing '{modName}'...";

			ProgressWrapper progress = new(
			new(value => App.Instance.Dispatcher.Invoke(() => ProgressDescription.Text = value)),
			new(value => App.Instance.Dispatcher.Invoke(() => ProgressBar.Value = value)));

			progress.Report("Downloading.", 0);

			using WebClient wc = new();
			byte[]? downloadedModContents = await wc.DownloadByteArrayAsync($"https://devildaggers.info/api/mods/{Uri.EscapeDataString(modName)}/file", progress, _cancellationTokenSource);
			if (downloadedModContents == null)
			{
				App.Instance.Dispatcher.Invoke(() => progress.Report("Download failed.", 0));
				return;
			}

			App.Instance.Dispatcher.Invoke(() => progress.Report("Installing.", 1));
			using MemoryStream ms = new(downloadedModContents);
			using ZipArchive zipArchive = new(ms);
			zipArchive.ExtractToDirectory(modsDirectory, true);
			App.Instance.Dispatcher.Invoke(() => progress.Report("Installation complete.", 1));

			ButtonOk.IsEnabled = true;
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			_cancellationTokenSource.Cancel();
		}
	}
}
