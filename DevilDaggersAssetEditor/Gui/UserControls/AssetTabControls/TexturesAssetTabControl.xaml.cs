using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code.AssetTabControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetControls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetTabControls
{
	public partial class TexturesAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(TexturesAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		internal TexturesAssetTabControlHandler Handler { get; private set; }

		public TexturesAssetTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new TexturesAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType, true));

			foreach (TextureAssetControl ac in Handler.CreateAssetControls())
				AssetEditor.Items.Add(ac);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TextureAssetControl ac = e.AddedItems[0] as TextureAssetControl;

			Handler.SelectAsset(ac.Handler.Asset);
			Previewer.Initialize(ac.Handler.Asset);
		}
	}

	internal class TexturesAssetTabControlHandler : AbstractAssetTabControlHandler<TextureAsset, TextureAssetControl>
	{
		protected override string AssetTypeJsonFileName => "Textures";

		internal TexturesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		internal override void UpdateGui(TextureAsset asset)
		{
			TextureAssetControl ac = assetControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			ac.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}