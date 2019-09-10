using DevilDaggersAssetEditor.Code.Assets;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.GUI.UserControls
{
	public partial class AudioAssetControl : UserControl
	{
		public AudioAsset AudioAsset { get; set; }

		public AudioAssetControl(AudioAsset audioAsset)
		{
			InitializeComponent();

			AudioAsset = audioAsset;

			Data.DataContext = AudioAsset;
		}

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog
			{
				Filter = ".wav"
			};
			bool? openResult = openDialog.ShowDialog();
			if (!openResult.HasValue || !openResult.Value)
				return;

			AudioAsset.EditorPath = openDialog.FileName;
		}
	}
}