﻿using System.Windows;

namespace DevilDaggersAssetEditor.New.Wpf.Gui.Windows
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			App.Instance.MainWindow = this;
			App.Instance.UpdateMainWindowTitle();
		}
	}
}