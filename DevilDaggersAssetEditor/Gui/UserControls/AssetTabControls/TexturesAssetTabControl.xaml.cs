using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
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

		public TexturesAssetTabControlHandler Handler { get; private set; }

		public TexturesAssetTabControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new TexturesAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType, true));

			foreach (TextureAssetRowControl arc in Handler.CreateAssetRowControls())
				AssetEditor.Items.Add(arc);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TextureAssetRowControl arc = e.AddedItems[0] as TextureAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}
	}

	public class TexturesAssetTabControlHandler : AbstractAssetTabControlHandler<TextureAsset, TextureAssetRowControl>
	{
		protected override string AssetTypeJsonFileName => "Textures";

		public TexturesAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGui(TextureAsset asset)
		{
			TextureAssetRowControl arc = assetRowControls.Where(a => a.Handler.Asset == asset).FirstOrDefault();
			arc.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}