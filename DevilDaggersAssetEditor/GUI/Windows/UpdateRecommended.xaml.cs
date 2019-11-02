﻿using DevilDaggersAssetEditor.Code.Network;
using DevilDaggersCore.Tools;
using System.Diagnostics;
using System.Windows;

namespace DevilDaggersAssetEditor.GUI.Windows
{
	public partial class UpdateRecommendedWindow : Window
	{
		public UpdateRecommendedWindow()
		{
			InitializeComponent();

			Text.Content = $"{App.ApplicationDisplayName} {NetworkHandler.Instance.VersionResult.Tool.VersionNumber} is available. The current version is {App.LocalVersion}.";
		}

		private void DownloadButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(UrlUtils.ApiGetTool(App.ApplicationName));
			Close();
		}
	}
}