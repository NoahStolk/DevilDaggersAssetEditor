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

		private readonly List<CheckBox> checkBoxes = new List<CheckBox>();

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
			FilterOperationAnd.Checked += (sender, e) => ApplyFilter();
			FilterOperationOr.Checked += (sender, e) => ApplyFilter();

			IEnumerable<string> tags = Handler.Assets.SelectMany(a => a.Tags).Distinct().OrderBy(s => s);
			int cols = 5;
			int i = 0;
			List<StackPanel> stackPanels = new List<StackPanel>();
			for (; i < cols; i++)
			{
				Filters.ColumnDefinitions.Add(new ColumnDefinition());
				StackPanel stackPanel = new StackPanel { Tag = i };
				Grid.SetColumn(stackPanel, i);
				Filters.Children.Add(stackPanel);
				stackPanels.Add(stackPanel);
			}

			i = 0;
			foreach (string tag in tags)
			{
				int pos = (int)(i++ / (float)tags.Count() * cols);
				CheckBox checkBox = new CheckBox { Content = tag };
				checkBox.Checked += (sender, e) => ApplyFilter();
				checkBox.Unchecked += (sender, e) => ApplyFilter();
				checkBoxes.Add(checkBox);
				stackPanels.FirstOrDefault(s => (int)s.Tag == pos).Children.Add(checkBox);
			}
		}

		private void ApplyFilter()
		{
			FilterOperation filterOperation = GetFilterOperation();
			IEnumerable<string> checkedFiters = checkBoxes.Where(c => c.IsChecked.Value).Select(s => s.Content.ToString());
			if (checkedFiters.Count() == 0)
			{
				foreach (TextureAssetRowControl arc in Handler.assetRowControls)
					arc.Visibility = Visibility.Visible;
			}
			else
			{
				foreach (TextureAssetRowControl arc in Handler.assetRowControls)
				{
					TextureAsset asset = Handler.Assets.FirstOrDefault(a => a == arc.Handler.Asset);
					if (asset != null)
					{
						switch (filterOperation)
						{
							case FilterOperation.And:
								arc.Visibility = checkedFiters.All(t => asset.Tags.Contains(t)) ? Visibility.Visible : Visibility.Collapsed;
								break;
							case FilterOperation.Or:
								arc.Visibility = asset.Tags.Any(t => checkedFiters.Contains(t)) ? Visibility.Visible : Visibility.Collapsed;
								break;
						}
					}
				}
			}
		}

		private FilterOperation GetFilterOperation()
		{
			if (FilterOperationAnd.IsChecked.Value)
				return FilterOperation.And;
			if (FilterOperationOr.IsChecked.Value)
				return FilterOperation.Or;
			return FilterOperation.None;
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
			TextureAssetRowControl arc = assetRowControls.FirstOrDefault(a => a.Handler.Asset == asset);
			arc.TextBlockEditorPath.Text = asset.EditorPath;
			arc.Handler.UpdateGui();
		}
	}
}