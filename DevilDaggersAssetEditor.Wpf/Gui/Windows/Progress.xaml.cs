﻿using DevilDaggersAssetEditor.Wpf.Native;
using System;
using System.Windows;
using System.Windows.Interop;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class ProgressWindow : Window
	{
#pragma warning disable IDE1006
#pragma warning disable SA1310 // Field names should not contain underscore
		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;
#pragma warning restore IDE1006
#pragma warning restore SA1310 // Field names should not contain underscore

		public ProgressWindow(string title)
		{
			Title = title;

			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Removes Exit button.
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
#pragma warning disable CA1806 // Do not ignore method results
			NativeMethods.SetWindowLong(hwnd, GWL_STYLE, NativeMethods.GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
#pragma warning restore CA1806 // Do not ignore method results
		}

		private void OkButton_Click(object sender, RoutedEventArgs e) => Close();

		public void Finish()
		{
			ProgressBar.Value = 1;
			ProgressDescription.Text = "Completed successfully.";
			OkButton.Visibility = Visibility.Visible;
		}

		public void Error()
		{
			ProgressDescription.Text = "Execution did not complete successfully.";
			OkButton.Visibility = Visibility.Visible;
		}
	}
}