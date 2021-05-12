using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class RenameFileWindow : Window
	{
		public RenameFileWindow(string currentFileName)
		{
			InitializeComponent();

			TextBoxFileName.Text = currentFileName;
			TextBoxFileName.Focus();
			TextBoxFileName.SelectAll();
		}

		public string? NewFileName { get; private set; }

		public void OkButton_Click(object sender, RoutedEventArgs e)
		{
			NewFileName = TextBoxFileName.Text;
			Close();
		}
	}
}
