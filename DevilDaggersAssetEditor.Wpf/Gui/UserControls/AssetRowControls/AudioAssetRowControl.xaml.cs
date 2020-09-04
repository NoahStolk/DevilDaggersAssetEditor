using DevilDaggersAssetEditor.Wpf.Code.RowControlHandlers;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls.AssetRowControls
{
	public partial class AudioAssetRowControl : UserControl
	{
		public AudioAssetRowControl(AudioAssetRowControlHandler handler)
		{
			Handler = handler;

			InitializeComponent();

			Data.Children.Add(Handler.TextBlockTags);
			Data.Children.Add(Handler.RectangleInfo);
			Data.Children.Add(Handler.RectangleEdit);

			Data.DataContext = Handler.Asset;
		}

		public AudioAssetRowControlHandler Handler { get; }

		private static bool ValidateTextBoxLoudness(TextBox textBox)
		{
			bool isValid = float.TryParse(textBox.Text, out float res) && res >= 0;

			textBox.Background = isValid ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(255, 127, 127));

			return isValid;
		}

		private void TextBoxLoudness_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBoxLoudness(TextBoxLoudness))
				Handler.Asset.Loudness = float.Parse(TextBoxLoudness.Text, CultureInfo.InvariantCulture);
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