using DevilDaggersAssetEditor.Assets;
using DevilDaggersCore.Wpf.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls
{
	public partial class AudioAssetRowControl : UserControl
	{
		public AudioAssetRowControl(AssetRowControlHandler handler)
		{
			Handler = handler;

			InitializeComponent();

			Data.Children.Add(Handler.TextBlockTags);
			Data.Children.Add(Handler.RectangleInfo);
			Data.Children.Add(Handler.RectangleEdit);

			Data.DataContext = Handler.Asset;

			// Update GUI from row control handlers
			TextBoxLoudness.Text = (Handler.Asset as AudioAsset).Loudness.ToString(CultureInfo.InvariantCulture);
		}

		public AssetRowControlHandler Handler { get; }

		private static bool ValidateTextBoxLoudness(TextBox textBox)
		{
			bool isValid = float.TryParse(textBox.Text, out float parsed) && parsed >= 0;

			textBox.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];

			return isValid;
		}

		private void TextBoxLoudness_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBoxLoudness(TextBoxLoudness))
				(Handler.Asset as AudioAsset).Loudness = float.Parse(TextBoxLoudness.Text, CultureInfo.InvariantCulture);
		}

		private void ButtonRemovePath_Click(object sender, RoutedEventArgs e)
			=> Handler.RemovePath();

		private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
			=> Handler.BrowsePath();

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
			=> TextBoxLoudness.TextChanged += TextBoxLoudness_TextChanged;

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
			=> Handler.UpdateGui();
	}
}