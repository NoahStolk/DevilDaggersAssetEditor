using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.Binaries;
using DevilDaggersAssetEditor.Extensions;
using DevilDaggersAssetEditor.ModFiles;
using DevilDaggersAssetEditor.Utils;
using DevilDaggersAssetEditor.Wpf.Gui.UserControls.PreviewerControls;
using DevilDaggersAssetEditor.Wpf.ModFiles;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Extensions;
using DevilDaggersCore.Mods;
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
		private readonly AssetRowSorting _nameSort = new(a => a.Asset.AssetName);
		private readonly AssetRowSorting _prohibitedSort = new(a => a.Asset.IsProhibited);
		private readonly AssetRowSorting _tagsSort = new(a => string.Join(", ", a.Asset.Tags));
		private readonly AssetRowSorting _descriptionSort = new(a => a.Asset.Description ?? "Not fetched");
		private readonly AssetRowSorting _loudnessSort = new(a => (a.Asset as AudioAsset)?.Loudness ?? 0);
		private readonly AssetRowSorting _pathSort = new(a => a.Asset.EditorPath);

		private AssetRowSorting _activeSorting = new(a => a.Asset.AssetName);

		public AssetTabControl(BinaryType binaryType, AssetType assetType, string openDialogFilter, string assetTypeJsonFileName)
		{
			InitializeComponent();

			AssetType = assetType;

			List<AbstractAsset> assets = AssetContainer.Instance.GetAssets(binaryType, assetTypeJsonFileName);

			int i = 0;
			foreach (AbstractAsset asset in assets)
			{
				AssetRowControl assetRowControl = new(asset, assetType, i++ % 2 == 0, openDialogFilter);
				UserAsset? userAsset = ModFileHandler.Instance.ModFile.Find(ua => ua.AssetType == assetType && ua.AssetName == asset.AssetName);
				assetRowControl.Asset.EditorPath = userAsset?.EditorPath ?? GuiUtils.FileNotFound;
				if (asset.AssetType == AssetType.Shader && userAsset is ShaderUserAsset shaderUserAsset && assetRowControl.ShaderAsset != null)
					assetRowControl.ShaderAsset.EditorPathFragmentShader = shaderUserAsset?.EditorPathFragmentShader ?? GuiUtils.FileNotFound;

				assetRowControl.UpdateGui();
				RowControls.Add(assetRowControl);
			}

			AllFilters = RowControls.Select(a => a.Asset).SelectMany(a => a.Tags ?? new()).Where(t => !string.IsNullOrEmpty(t)).Distinct().OrderBy(s => s);
			FiltersCount = AllFilters.Count();

			FilterHighlightColor = EditorUtils.FromRgbTuple(assetType.GetColor()) * 0.25f;

			Previewer = assetType switch
			{
				AssetType.Audio => new AudioPreviewerControl(),
				AssetType.Model => new ModelPreviewerControl(),
				AssetType.ModelBinding => new ModelBindingPreviewerControl(),
				AssetType.Shader => new ShaderPreviewerControl(),
				AssetType.Texture => new TexturePreviewerControl(),
				_ => throw new NotSupportedException($"Previewer control for type {assetType} is not supported."),
			};

			MainGrid.Children.Add(Previewer);

			if (assetType == AssetType.Audio)
			{
				ColumnDefinitionLoudness.Width = new GridLength(1, GridUnitType.Star);
				ColumnDefinitionPath.Width = new GridLength(7, GridUnitType.Star);
			}
		}

		private enum FilterOperation
		{
			None,
			And,
			Or,
		}

		public AssetType AssetType { get; }

		public UserControl Previewer { get; }

		public List<AssetRowControl> RowControls { get; } = new();
		public AbstractAsset? SelectedAsset { get; set; }

		public List<CheckBox> FilterCheckBoxes { get; } = new();
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
				Filters.ColumnDefinitions.Add(new());
			for (int i = 0; i < rows; i++)
				Filters.RowDefinitions.Add(new());

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
				AssetRowControl arc = AssetEditor.Items.OfType<AssetRowControl>().First(arc => arc.Asset == sorted[i].Asset);
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

		private void ProhibitedSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_prohibitedSort);

		private void TagsSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_tagsSort);

		private void DescriptionSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_descriptionSort);

		private void LoudnessSortButton_Click(object sender, RoutedEventArgs e)
			=> SetSorting(_loudnessSort);

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
			foreach (AssetRowControl assetRowControl in RowControls.Where(a => a.IsActive))
				assetRowControl.UpdateTagHighlighting(CheckedFilters, FilterHighlightColor);
		}

		public void SetAssetEditorBackgroundColors(ItemCollection items)
		{
			foreach (AssetRowControl assetRowControl in RowControls.Where(a => a.IsActive))
				assetRowControl.UpdateBackgroundRectangleColors(items.IndexOf(assetRowControl) % 2 == 0);
		}

		public void SelectAsset(AbstractAsset asset)
			=> SelectedAsset = asset;

		public void ImportFolder(string folder, AssetType assetType, SearchOption searchOption)
		{
			if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
				return;

			foreach (string filePath in Directory.GetFiles(folder, $"*{assetType.GetFileExtension()}", searchOption))
			{
				string assetName = Path.GetFileNameWithoutExtension(filePath);
				bool isFragmentShader = false;
				if (assetType == AssetType.Shader)
				{
					isFragmentShader = assetName.EndsWith("_fragment", StringComparison.InvariantCulture);
					assetName = assetName.TrimEnd("_fragment").TrimEnd("_vertex");
				}

				AssetRowControl? assetRowControl = RowControls.Find(a => a.Asset.AssetName == assetName);
				if (assetRowControl == null)
					continue;

				if (isFragmentShader && assetRowControl.Asset is ShaderAsset shaderAsset)
					shaderAsset.EditorPathFragmentShader = filePath;
				else
					assetRowControl.Asset.EditorPath = filePath;
				assetRowControl.UpdateGui();
			}
		}

		public void CreateFiltersGui(int rows)
		{
			int i = 0;
			foreach (string tag in AllFilters)
			{
				CheckBox checkBox = new() { Content = tag };
				Grid.SetColumn(checkBox, i / rows);
				Grid.SetRow(checkBox, i % rows);
				FilterCheckBoxes.Add(checkBox);
				i++;
			}
		}

		private void ApplyFilter(object sender, RoutedEventArgs e)
		{
			ApplyFilter(GetFilterOperation());

			foreach (AssetRowControl assetRowControl in RowControls)
			{
				if (!assetRowControl.IsActive)
				{
					if (AssetEditor.Items.Contains(assetRowControl))
						AssetEditor.Items.Remove(assetRowControl);
				}
				else
				{
					if (!AssetEditor.Items.Contains(assetRowControl))
						AssetEditor.Items.Add(assetRowControl);
				}
			}

			ApplySort();
		}

		private void ApplyFilter(FilterOperation filterOperation)
		{
			foreach (AssetRowControl assetRowControl in RowControls)
			{
				if (!CheckedFilters.Any())
				{
					assetRowControl.IsActive = true;
				}
				else
				{
					assetRowControl.IsActive = filterOperation switch
					{
						FilterOperation.And => CheckedFilters.All(t => assetRowControl.Asset.Tags.Contains(t)),
						FilterOperation.Or => assetRowControl.Asset.Tags.Any(t => CheckedFilters.Contains(t)),
						_ => throw new NotSupportedException($"{nameof(FilterOperation)} {filterOperation} not supported in {nameof(ApplyFilter)} method."),
					};
				}
			}

			UpdateTagHighlighting();
		}

		public List<AbstractAsset> GetAssets()
			=> RowControls.ConvertAll(a => a.Asset);

		public void UpdateAssetTabControls(List<UserAsset> modFile)
		{
			foreach (AssetRowControl assetRowControl in RowControls)
			{
				AbstractAsset asset = assetRowControl.Asset;
				UserAsset? userAsset = modFile.Find(a => a.AssetName == asset.AssetName && a.AssetType == asset.AssetType);
				if (userAsset != null)
				{
					asset.ImportValuesFromUserAsset(userAsset);

					assetRowControl.UpdateGui();
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
