using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System;
using DevilDaggersAssetEditor.Code.AssetTabControlHandlers;

namespace DevilDaggersAssetEditor.GUI.UserControls.AssetTabControls
{
	public partial class ModelsAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(ModelsAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public ModelsAssetTabControlHandler Handler { get; private set; }

		public ModelsAssetTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new ModelsAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (ModelAssetControl ac in Handler.CreateAssetControls())
				AssetEditor.Items.Add(ac);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ModelAssetControl ac = e.AddedItems[0] as ModelAssetControl;

			Handler.SelectAsset(ac.Handler.Asset);
			Previewer.Initialize(ac.Handler.Asset);
		}
	}
}