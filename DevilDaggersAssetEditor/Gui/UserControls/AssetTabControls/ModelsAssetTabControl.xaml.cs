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

			Handler = new ModelsAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType, true));

			foreach (ModelAssetRowControl arc in Handler.CreateAssetRowControls())
				AssetEditor.Items.Add(arc);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ModelAssetRowControl arc = e.AddedItems[0] as ModelAssetRowControl;

			Handler.SelectAsset(arc.Handler.Asset);
			Previewer.Initialize(arc.Handler.Asset);
		}
	}

	public class ModelsAssetTabControlHandler : AbstractAssetTabControlHandler<ModelAsset, ModelAssetRowControl>
	{
		protected override string AssetTypeJsonFileName => "Models";

		public ModelsAssetTabControlHandler(BinaryFileType binaryFileType)
			: base(binaryFileType)
		{
		}

		public override void UpdateGui(ModelAsset asset)
		{
			ModelAssetRowControl arc = assetRowControls.FirstOrDefault(a => a.Handler.Asset == asset);
			arc.TextBlockEditorPath.Text = asset.EditorPath;
		}
	}
}