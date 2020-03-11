using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DevilDaggersAssetEditor.Gui.Windows
{
	public partial class ProgressWindow : Window
	{
		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		public ProgressWindow(string title)
		{
			Title = title;

			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Removes Exit button.
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		public void Finish()
		{
			ProgressBar.Value = 1;
			ProgressDescription.Text = "Completed successfully.";
			OKButton.Visibility = Visibility.Visible;
		}

		public void Error()
		{
			ProgressDescription.Text = "Execution failed.";
			OKButton.Visibility = Visibility.Visible;
		}
	}
}