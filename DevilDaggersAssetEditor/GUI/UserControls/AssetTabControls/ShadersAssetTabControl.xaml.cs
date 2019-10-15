using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System;
using DevilDaggersAssetEditor.Code.AssetTabControlHandlers;

namespace DevilDaggersAssetEditor.GUI.UserControls.AssetTabControls
{
	public partial class ShadersAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(ShadersAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public ShadersAssetTabControlHandler Handler { get; private set; }

		public ShadersAssetTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new ShadersAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (ShaderAssetControl ac in Handler.CreateAssetControls())
				AssetEditor.Items.Add(ac);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ShaderAssetControl ac = e.AddedItems[0] as ShaderAssetControl;

			Handler.SelectAsset(ac.Handler.Asset);
			Previewer.Initialize(ac.Handler.Asset);
		}
	}
}