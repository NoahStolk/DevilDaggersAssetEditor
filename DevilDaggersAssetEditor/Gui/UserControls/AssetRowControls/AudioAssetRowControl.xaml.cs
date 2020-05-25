using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls
{
	public partial class AudioAssetRowControl : UserControl
	{
		public AudioAssetRowControlHandler Handler { get; private set; }

		public AudioAssetRowControl(AudioAsset asset, bool isEven)
		{
			InitializeComponent();
			TextBlockTags.Text = asset.Tags != null ? string.Join(", ", asset.Tags) : string.Empty;

			Handler = new AudioAssetRowControlHandler(asset, this, isEven);

			Data.Children.Add(Handler.rectangleInfo);
			Data.Children.Add(Handler.rectangleEdit);

			Handler.UpdateBackgroundRectangleColors(isEven);

			Data.DataContext = asset;
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e) => Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e) => Handler.BrowsePath();

		private bool ValidateTextBoxLoudness(TextBox textBox)
		{
			bool isValid = float.TryParse(textBox.Text, out float res) && res >= 0;

			textBox.Background = isValid ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(255, 127, 127));

			return isValid;
		}

		private void TextBoxLoudness_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBoxLoudness(TextBoxLoudness))
				Handler.Asset.Loudness = float.Parse(TextBoxLoudness.Text);
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e) => TextBoxLoudness.TextChanged += TextBoxLoudness_TextChanged;
	}

	public class AudioAssetRowControlHandler : AbstractAssetRowControlHandler<AudioAsset, AudioAssetRowControl>
	{
		public AudioAssetRowControlHandler(AudioAsset asset, AudioAssetRowControl parent, bool isEven)
			: base(asset, parent, "Audio files (*.wav)|*.wav", isEven)
		{
		}

		public override void UpdateGui()
		{
			bool isPathValid = File.Exists(Asset.EditorPath);
			parent.TextBlockEditorPath.Text = isPathValid ? Asset.EditorPath : Utils.FileNotFound;

			parent.TextBoxLoudness.Text = Asset.Loudness.ToString();
		}
	}
}