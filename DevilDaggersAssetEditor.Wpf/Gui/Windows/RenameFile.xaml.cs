using DevilDaggersCore.Wpf.Windows;
using System.IO;
using System.Windows;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows;

public partial class RenameFileWindow : Window
{
	private readonly string _directory;
	private readonly string _currentFileName;

	public RenameFileWindow(string directory, string currentFileName)
	{
		InitializeComponent();

		_directory = directory;
		_currentFileName = currentFileName;

		TextBoxFileName.Text = currentFileName;
		TextBoxFileName.Focus();
		TextBoxFileName.SelectAll();
	}

	public string? NewFileName { get; private set; }

	public void OkButton_Click(object sender, RoutedEventArgs e)
	{
		if (TextBoxFileName.Text == _currentFileName)
		{
			Close();
			return;
		}

		if (File.Exists(Path.Combine(_directory, TextBoxFileName.Text)))
		{
			MessageWindow window = new("File already exists", $"File '{TextBoxFileName.Text}' already exists in directory '{_directory}'.");
			window.ShowDialog();
		}
		else
		{
			NewFileName = TextBoxFileName.Text;
			Close();
		}
	}
}