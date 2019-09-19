using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetControlHandlers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.GUI.UserControls.AssetControls
{
	public partial class AudioAssetControl : UserControl
	{
		public AudioAssetControlHandler Handler { get; private set; }

		public AudioAssetControl(AudioAsset asset)
		{
			InitializeComponent();

			Handler = new AudioAssetControlHandler(asset, this);

			Data.DataContext = asset;
		}

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
		{
			Handler.BrowsePath();
		}

		private bool ValidateTextBox(TextBox textBox)
		{
			bool valid = float.TryParse(textBox.Text, out _);

			textBox.Background = valid ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(255, 128, 128));

			return valid;
		}

		private void TextBoxLoudness_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBox(TextBoxLoudness))
				Handler.Asset.Loudness = float.Parse(TextBoxLoudness.Text);
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			TextBoxLoudness.TextChanged += TextBoxLoudness_TextChanged;
		}
	}
}