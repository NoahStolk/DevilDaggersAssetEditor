using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Extensions;
using DevilDaggersCore.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class AssetTabControl : UserControl
	{
		private readonly AssetRowSorting _nameSort = new AssetRowSorting((a) => a.Asset.AssetName);
		private readonly AssetRowSorting _tagsSort = new AssetRowSorting((a) => string.Join(", ", a.Asset.Tags));
		private readonly AssetRowSorting _descriptionSort = new AssetRowSorting((a) => a.Asset.Description);
		private readonly AssetRowSorting _pathSort = new AssetRowSorting((a) => a.Asset.EditorPath);

		private AssetRowSorting _activeSorting = new AssetRowSorting((a) => a.Asset.AssetName);

		public AssetTabControl(BinaryFileType binaryFileType, AssetType assetType, string openDialogFilter, string assetTypeJsonFileName)
		{
			InitializeComponent();

			List<AbstractAsset> assets = AssetHandler.Instance.GetAssets(binaryFileType, assetTypeJsonFileName).ToList();

			int i = 0;
			foreach (AbstractAsset asset in assets)
			{
				AssetRowControl rowHandler = new AssetRowControl(asset, assetType, i++ % 2 == 0, openDialogFilter);
				RowControls.Add(rowHandler);
			}

			AllFilters = RowControls.Select(a => a.Asset).SelectMany(a => a.Tags ?? new List<string>()).Where(t => !string.IsNullOrEmpty(t)).Distinct().OrderBy(s => s);
			FiltersCount = AllFilters.Count();

			FilterHighlightColor = EditorUtils.FromRgbTuple(assetType.GetColorFromAssetType()) * 0.25f;

			Previewer = assetType switch
			{
				AssetType.Audio => new AudioPreviewerControl(),
				AssetType.Model => new ModelPreviewerControl(),
				AssetType.ModelBinding => new ModelBindingPreviewerControl(),
				AssetType.Particle => new ParticlePreviewerControl(),
				AssetType.Shader => new ShaderPreviewerControl(),
				AssetType.Texture => new TexturePreviewerControl(),
				_ => throw new NotSupportedException($"Previewer control for type {assetType} is not supported."),
			};

			MainGrid.Children.Add(Previewer);
		}

		private enum FilterOperation
		{
			None,
			And,
			Or,
		}

		public UserControl Previewer { get; }

		public List<AssetRowControl> RowControls { get; } = new List<AssetRowControl>();
		public AbstractAsset? SelectedAsset { get; set; }

		public List<CheckBox> FilterCheckBoxes { get; } = new List<CheckBox>();
		public Color FilterHighlightColor { get; private set; }

		public IEnumerable<string> CheckedFilters => FilterCheckBoxes.Where(c => c.IsChecked()).Select(s => s.Content.ToString() ?? string.Empty);
		public IEnumerable<string> AllFilters { get; }
		public int FiltersCount { get; }

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			foreach (AssetRowControl arc in RowControls)
				AssetEditor.Items.Add(arc);

			CreateFiltersGui();
		}

		private void CreateFiltersGui()
		{
			FilterOperationAnd.Checked += ApplyFilter;
			FilterOperationOr.Checked += ApplyFilter;

			const int columns = 9;
			int rows = (int)Math.Ceiling(FiltersCount / (double)columns);
			CreateFiltersGui(rows);

			for (int i = 0; i < columns; i++)
				Filters.ColumnDefinitions.Add(new ColumnDefinition());
			for (int i = 0; i < rows; i++)
				Filters.RowDefinitions.Add(new RowDefinition());

			foreach (CheckBox checkBox in FilterCheckBoxes)
			{
				checkBox.Checked += ApplyFilter;
				checkBox.Unchecked += ApplyFilter;
				Filters.Children.Add(checkBox);
			}
		}

		private void ApplySort()
		{
			List<AssetRowControl> sorted = SortRowControlHandlers();
			for (int i = 0; i < sorted.Count; i++)
			{
				AssetRowControl arc = AssetEditor.Items.OfType<AssetRowControl>().FirstOrDefault(arc => arc.Asset == sorted[i].Asset);
				AssetEditor.Items.Remove(arc);
				AssetEditor.Items.Insert(i, arc);
			}

			SetAssetEditorBackgroundColors(AssetEditor.Items);
		}

		public List<AssetRowControl> SortRowControlHandlers()
		{
			IEnumerable<AssetRowControl> query = RowControls.Where(a => a.IsActive);
			query = _activeSorting.IsAscending ? query.OrderBy(_activeSorting.SortingFunction) : query.OrderByDescending(_activeSorting.SortingFunction);
			return query.ToList();
		}

		private FilterOperation GetFilterOperation()
		{
			if (FilterOperationAnd.IsChecked())
				return FilterOperation.And;
			if (FilterOperationOr.IsChecked())
				return FilterOperation.Or;
			return FilterOperation.None;
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

			if (e.AddedItems[0] is AssetRowControl arc)
			{
				SelectAsset(arc.Asset);
				if (Previewer is IPreviewerControl previewerControl)
					previewerControl.Initialize(arc.Asset);
			}
		}

		private void NameSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_nameSort);

		private void TagsSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_tagsSort);

		private void DescriptionSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_descriptionSort);

		private void PathSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_pathSort);

		private void SetSorting(AssetRowSorting sorting)
		{
			sorting.IsAscending = !sorting.IsAscending;
			_activeSorting = sorting;

			ApplySort();
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
			=> UpdateTagHighlighting();

		public void UpdateTagHighlighting()
		{
			foreach (AssetRowControl rowHandler in RowControls.Where(a => a.IsActive))
				rowHandler.UpdateTagHighlighting(CheckedFilters, FilterHighlightColor);
		}

		public void SetAssetEditorBackgroundColors(ItemCollection items)
		{
			foreach (AssetRowControl rowHandler in RowControls.Where(a => a.IsActive))
				rowHandler.UpdateBackgroundRectangleColors(items.IndexOf(rowHandler) % 2 == 0);
		}

		public void SelectAsset(AbstractAsset asset)
			=> SelectedAsset = asset;

		public void ImportFolder(string folder, AssetType assetType)
		{
			if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
				return;

			foreach (string filePath in Directory.GetFiles(folder, $"*{assetType.GetFileExtensionFromAssetType()}", SearchOption.AllDirectories))
			{
				string assetName = Path.GetFileNameWithoutExtension(filePath);
				bool isFragmentShader = false;
				if (assetType == AssetType.Shader)
				{
					isFragmentShader = assetName.EndsWith("_fragment", StringComparison.InvariantCulture);
					assetName = assetName.TrimEnd("_fragment").TrimEnd("_vertex");
				}

				AssetRowControl? rowControl = RowControls.Find(a => a.Asset.AssetName == assetName);
				if (rowControl == null)
					continue;

				if (isFragmentShader && rowControl.Asset is ShaderAsset shaderAsset)
					shaderAsset.EditorPathFragmentShader = filePath;
				else
					rowControl.Asset.EditorPath = filePath;
				rowControl.UpdateGui();
			}
		}

		public void CreateFiltersGui(int rows)
		{
			int i = 0;
			foreach (string tag in AllFilters)
			{
				CheckBox checkBox = new CheckBox { Content = tag };
				Grid.SetColumn(checkBox, i / rows);
				Grid.SetRow(checkBox, i % rows);
				FilterCheckBoxes.Add(checkBox);
				i++;
			}
		}

		private void ApplyFilter(object sender, RoutedEventArgs e)
		{
			ApplyFilter(GetFilterOperation());

			foreach (AssetRowControl assetRowEntry in RowControls)
			{
				if (!assetRowEntry.IsActive)
				{
					if (AssetEditor.Items.Contains(assetRowEntry))
						AssetEditor.Items.Remove(assetRowEntry);
				}
				else
				{
					if (!AssetEditor.Items.Contains(assetRowEntry))
						AssetEditor.Items.Add(assetRowEntry);
				}
			}

			ApplySort();
		}

		private void ApplyFilter(FilterOperation filterOperation)
		{
			foreach (AssetRowControl rowHandler in RowControls)
			{
				if (!CheckedFilters.Any())
				{
					rowHandler.IsActive = true;
				}
				else
				{
					rowHandler.IsActive = filterOperation switch
					{
						FilterOperation.And => CheckedFilters.All(t => rowHandler.Asset.Tags.Contains(t)),
						FilterOperation.Or => rowHandler.Asset.Tags.Any(t => CheckedFilters.Contains(t)),
						_ => throw new NotSupportedException($"{nameof(FilterOperation)} {filterOperation} not supported in {nameof(ApplyFilter)} method.")
					};
				}
			}

			UpdateTagHighlighting();
		}

		public List<AbstractAsset> GetAssets()
			=> RowControls.Select(a => a.Asset).ToList();

		public void UpdateAssetTabControls(List<UserAsset> userAssets)
		{
			foreach (AssetRowControl rowControl in RowControls)
			{
				AbstractAsset asset = rowControl.Asset;
				UserAsset? userAsset = userAssets.Find(a => a.AssetName == asset.AssetName && a.AssetType == asset.AssetType);
				if (userAsset != null)
				{
					asset.ImportValuesFromUserAsset(userAsset);

					rowControl.UpdateGui();
				}
			}
		}

		private class AssetRowSorting
		{
			public AssetRowSorting(Func<AssetRowControl, object> sortingFunction)
			{
				SortingFunction = sortingFunction;
			}

			public Func<AssetRowControl, object> SortingFunction { get; set; }
			public bool IsAscending { get; set; } = true;
		}
	}
}