using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Gui.UserControls.AssetRowControls;
using System;
using System.Collections.Generic;
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

			CreateFiltersGui();
		}

		private void CreateFiltersGui()
		{
			FilterOperationAnd.Checked += ApplyFilter;
			FilterOperationOr.Checked += ApplyFilter;

			Handler.CreateFiltersGui();

			foreach (StackPanel stackPanel in Handler.filterStackPanels)
			{
				Filters.ColumnDefinitions.Add(new ColumnDefinition());
				Filters.Children.Add(stackPanel);
			}

			foreach (CheckBox checkBox in Handler.filterCheckBoxes)
			{
				checkBox.Checked += ApplyFilter;
				checkBox.Unchecked += ApplyFilter;
			}
		}

		private void ApplyFilter(object sender, RoutedEventArgs e)
		{
			Handler.ApplyFilter(GetFilterOperation(), Handler.assetRowControls.Select(a => new KeyValuePair<TextureAssetRowControl, TextureAsset>(a, a.Handler.Asset)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

			foreach (KeyValuePair<TextureAssetRowControl, bool> kvp in Handler.assetRowControlActiveDict)
			{
				if (!kvp.Value)
				{
					if (AssetEditor.Items.Contains(kvp.Key))
						AssetEditor.Items.Remove(kvp.Key);
				}
				else
				{
					if (!AssetEditor.Items.Contains(kvp.Key))
						AssetEditor.Items.Add(kvp.Key);
				}
			}

			SetAssetEditorBackgroundColors();
		}

		private FilterOperation GetFilterOperation()
		{
			if (FilterOperationAnd.IsChecked.Value)
				return FilterOperation.And;
			if (FilterOperationOr.IsChecked.Value)
				return FilterOperation.Or;
			return FilterOperation.None;
		}

		private void SetAssetEditorBackgroundColors()
		{
			List<TextureAssetRowControl> rows = AssetEditor.Items.OfType<TextureAssetRowControl>().ToList();
			foreach (TextureAssetRowControl row in rows)
				row.Handler.UpdateBackgroundRectangleColors(rows.IndexOf(row) % 2 == 0);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

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
			TextureAssetRowControl arc = assetRowControls.FirstOrDefault(a => a.Handler.Asset == asset);
			arc.TextBlockEditorPath.Text = asset.EditorPath;
			arc.Handler.UpdateGui();
		}
	}
}