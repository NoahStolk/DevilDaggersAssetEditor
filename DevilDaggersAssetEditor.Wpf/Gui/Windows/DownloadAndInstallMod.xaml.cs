using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Wpf.Extensions;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class DownloadAndInstallModWindow : Window
	{
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

			progress.Report("Downloading...", 0);

			using WebClient wc = new();
			byte[]? downloadedModContents = await wc.DownloadByteArrayAsync($"https://devildaggers.info/api/mods/{Uri.EscapeDataString(modName)}/file", progress);
			if (downloadedModContents == null)
			{
				App.Instance.Dispatcher.Invoke(() => progress.Report("Download failed.", 0));
				return;
			}

			App.Instance.Dispatcher.Invoke(() => progress.Report("Installing...", 1));
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
	}
}
