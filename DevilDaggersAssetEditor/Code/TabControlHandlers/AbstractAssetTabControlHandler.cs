using DevilDaggersAssetCore;
using DevilDaggersAssetCore.Assets;
using DevilDaggersAssetCore.Info;
using DevilDaggersAssetCore.User;
using DevilDaggersAssetEditor.Code.RowControlHandlers;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersAssetEditor.Code.TabControlHandlers
{
	public abstract class AbstractAssetTabControlHandler<TAsset, TAssetRowControl, TAssetRowControlHandler>
		where TAsset : AbstractAsset
		where TAssetRowControl : UserControl
		where TAssetRowControlHandler : AbstractAssetRowControlHandler<TAsset, TAssetRowControl>
	{
		protected abstract string AssetTypeJsonFileName { get; }

		public List<TAssetRowControlHandler> RowHandlers { get; private set; } = new List<TAssetRowControlHandler>();
		public TAsset SelectedAsset { get; set; }

		public readonly List<CheckBox> filterCheckBoxes = new List<CheckBox>();
		public Color FilterHighlightColor { get; private set; }

		public IEnumerable<string> CheckedFilters => filterCheckBoxes.Where(c => c.IsChecked.Value).Select(s => s.Content.ToString());
		public IEnumerable<string> AllFilters { get; }
		public int FiltersCount { get; }

		public AssetRowSorting<TAsset, TAssetRowControl, TAssetRowControlHandler> ActiveSorting { get; set; } = new AssetRowSorting<TAsset, TAssetRowControl, TAssetRowControlHandler>((a) => a.Asset.AssetName);

		private UserSettings Settings => UserHandler.Instance.settings;

		protected AbstractAssetTabControlHandler(BinaryFileType binaryFileType)
		{
			using StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.{binaryFileType.ToString().ToLower()}.{AssetTypeJsonFileName}.json"));
			List<TAsset> assets = JsonConvert.DeserializeObject<List<TAsset>>(sr.ReadToEnd());

			int i = 0;
			foreach (TAsset asset in assets)
			{
				TAssetRowControlHandler rowHandler = (TAssetRowControlHandler)Activator.CreateInstance(typeof(TAssetRowControlHandler), asset, i++ % 2 == 0);
				RowHandlers.Add(rowHandler);
			}
			AllFilters = RowHandlers.Select(a => a.Asset).SelectMany(a => a.Tags ?? (new string[] { })).Where(t => !string.IsNullOrEmpty(t)).Distinct().OrderBy(s => s);
			FiltersCount = AllFilters.Count();

			ChunkInfo chunkInfo = ChunkInfo.All.FirstOrDefault(c => c.AssetType == typeof(TAsset));
			FilterHighlightColor = chunkInfo.GetColor() * 0.25f;
		}

		public void UpdateTagHighlighting()
		{
			foreach (TAssetRowControlHandler rowHandler in RowHandlers.Where(a => a.IsActive))
				rowHandler.UpdateTagHighlighting(CheckedFilters, FilterHighlightColor);
		}

		public void SetAssetEditorBackgroundColors(ItemCollection items)
		{
			foreach (TAssetRowControlHandler rowHandler in RowHandlers.Where(a => a.IsActive))
				rowHandler.UpdateBackgroundRectangleColors(items.IndexOf(rowHandler.AssetRowControl) % 2 == 0);
		}

		public void SelectAsset(TAsset asset)
		{
			SelectedAsset = asset;
		}

		public void ImportFolder()
		{
			using CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
			if (Settings.EnableAssetsRootFolder)
				dialog.InitialDirectory = Settings.AssetsRootFolder;

			CommonFileDialogResult result = dialog.ShowDialog();
			if (result != CommonFileDialogResult.Ok)
				return;

			foreach (string filePath in Directory.GetFiles(dialog.FileName))
			{
				TAssetRowControlHandler rowHandler = RowHandlers.FirstOrDefault(a => a.Asset.AssetName == Path.GetFileNameWithoutExtension(filePath).Replace("_fragment", "").Replace("_vertex", ""));
				if (rowHandler == null)
					continue;

				rowHandler.Asset.EditorPath = filePath.Replace("_fragment", "").Replace("_vertex", "");
				rowHandler.UpdateGui();
			}
		}

		public bool IsComplete()
		{
			foreach (TAsset asset in RowHandlers.Select(a => a.Asset))
				if (!File.Exists(asset.EditorPath.Replace(".glsl", "_vertex.glsl")))
					return false;
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
				filterCheckBoxes.Add(checkBox);
				i++;
			}
		}

		public void ApplyFilter(FilterOperation filterOperation, Dictionary<TAssetRowControl, TextBlock> textBlocks)
		{
			foreach (TAssetRowControlHandler rowHandler in RowHandlers)
			{
				TextBlock textBlockTags = textBlocks.FirstOrDefault(kvp => kvp.Key == rowHandler.AssetRowControl).Value;

				if (CheckedFilters.Count() == 0)
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

		public List<TAssetRowControlHandler> ApplySort()
		{
			IEnumerable<TAssetRowControlHandler> query = RowHandlers.Where(a => a.IsActive);
			query = ActiveSorting.IsAscending ? query.OrderBy(ActiveSorting.SortingFunction) : query.OrderByDescending(ActiveSorting.SortingFunction);
			return query.ToList();
		}
	}
}