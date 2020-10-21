using DevilDaggersAssetEditor.Assets;
using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Info;
using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersAssetEditor.Wpf.RowControlHandlers;
using DevilDaggersCore.Wpf.Extensions;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Wpf.TabControlHandlers
{
	public class AssetTabControlHandler
	{
		public AssetTabControlHandler(BinaryFileType binaryFileType, AssetType assetType, string openDialogFilter, string assetTypeJsonFileName)
		{
			List<AbstractAsset> assets = AssetHandler.Instance.GetAssets(binaryFileType, assetTypeJsonFileName).ToList();

			int i = 0;
			foreach (AbstractAsset asset in assets)
			{
				AssetRowControlHandler rowHandler = new AssetRowControlHandler(asset, assetType, i++ % 2 == 0, openDialogFilter);
				RowHandlers.Add(rowHandler);
			}

			AllFilters = RowHandlers.Select(a => a.Asset).SelectMany(a => a.Tags ?? Array.Empty<string>()).Where(t => !string.IsNullOrEmpty(t)).Distinct().OrderBy(s => s);
			FiltersCount = AllFilters.Count();

			ChunkInfo chunkInfo = ChunkInfo.All.FirstOrDefault(c => c.AssetType == assetType);
			FilterHighlightColor = chunkInfo.GetColor() * 0.25f;
		}

		public List<AssetRowControlHandler> RowHandlers { get; } = new List<AssetRowControlHandler>();
		public AbstractAsset? SelectedAsset { get; set; }

		public List<CheckBox> FilterCheckBoxes { get; } = new List<CheckBox>();
		public Color FilterHighlightColor { get; private set; }

		public IEnumerable<string> CheckedFilters => FilterCheckBoxes.Where(c => c.IsChecked()).Select(s => s.Content.ToString() ?? string.Empty);
		public IEnumerable<string> AllFilters { get; }
		public int FiltersCount { get; }

		public AssetRowSorting ActiveSorting { get; set; } = new AssetRowSorting((a) => a.Asset.AssetName);

		public void UpdateTagHighlighting()
		{
			foreach (AssetRowControlHandler rowHandler in RowHandlers.Where(a => a.IsActive))
				rowHandler.UpdateTagHighlighting(CheckedFilters, FilterHighlightColor);
		}

		public void SetAssetEditorBackgroundColors(ItemCollection items)
		{
			foreach (AssetRowControlHandler rowHandler in RowHandlers.Where(a => a.IsActive))
				rowHandler.UpdateBackgroundRectangleColors(items.IndexOf(rowHandler.AssetRowControl) % 2 == 0);
		}

		public void SelectAsset(AbstractAsset asset)
			=> SelectedAsset = asset;

		public void ImportFolder()
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			if (UserHandler.Instance.Settings.EnableAssetsRootFolder && Directory.Exists(UserHandler.Instance.Settings.AssetsRootFolder))
				dialog.SelectedPath = $"{UserHandler.Instance.Settings.AssetsRootFolder}\\";

			if (dialog.ShowDialog() != true)
				return;

			foreach (string filePath in Directory.GetFiles(dialog.SelectedPath))
			{
				AssetRowControlHandler rowHandler = RowHandlers.FirstOrDefault(a => a.Asset.AssetName == Path.GetFileNameWithoutExtension(filePath).Replace("_fragment", string.Empty, StringComparison.InvariantCulture).Replace("_vertex", string.Empty, StringComparison.InvariantCulture));
				if (rowHandler == null)
					continue;

				rowHandler.Asset.EditorPath = filePath.Replace("_fragment", string.Empty, StringComparison.InvariantCulture).Replace("_vertex", string.Empty, StringComparison.InvariantCulture);
				rowHandler.UpdateGui();
			}
		}

		public bool IsComplete()
		{
			foreach (AbstractAsset asset in RowHandlers.Select(a => a.Asset))
			{
				if (!File.Exists(asset.EditorPath.Replace(".glsl", "_vertex.glsl", StringComparison.InvariantCulture)))
					return false;
			}

			return true;
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

		public void ApplyFilter(FilterOperation filterOperation)
		{
			foreach (AssetRowControlHandler rowHandler in RowHandlers)
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
						_ => throw new NotImplementedException($"{nameof(FilterOperation)} {filterOperation} not implemented in {nameof(ApplyFilter)} method.")
					};
				}
			}

			UpdateTagHighlighting();
		}

		public List<AssetRowControlHandler> ApplySort()
		{
			IEnumerable<AssetRowControlHandler> query = RowHandlers.Where(a => a.IsActive);
			query = ActiveSorting.IsAscending ? query.OrderBy(ActiveSorting.SortingFunction) : query.OrderByDescending(ActiveSorting.SortingFunction);
			return query.ToList();
		}
	}
}