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

		public List<AssetRowEntry<TAsset, TAssetRowControl, TAssetRowControlHandler>> AssetRowEntries { get; private set; } = new List<AssetRowEntry<TAsset, TAssetRowControl, TAssetRowControlHandler>>();
		public TAsset SelectedAsset { get; set; }

		public readonly List<StackPanel> filterStackPanels = new List<StackPanel>();
		public readonly List<CheckBox> filterCheckBoxes = new List<CheckBox>();
		public Color FilterHighlightColor { get; private set; }

		public IEnumerable<string> CheckedFilters => filterCheckBoxes.Where(c => c.IsChecked.Value).Select(s => s.Content.ToString());

		public AssetRowSorting<TAsset, TAssetRowControl, TAssetRowControlHandler> ActiveSorting { get; set; } = new AssetRowSorting<TAsset, TAssetRowControl, TAssetRowControlHandler>((a) => a.AssetRowControlHandler.Asset.AssetName);

		private UserSettings Settings => UserHandler.Instance.settings;

		protected AbstractAssetTabControlHandler(BinaryFileType binaryFileType)
		{
			using StreamReader sr = new StreamReader(Utils.GetAssemblyByName("DevilDaggersAssetCore").GetManifestResourceStream($"DevilDaggersAssetCore.Content.{binaryFileType.ToString().ToLower()}.{AssetTypeJsonFileName}.json"));
			List<TAsset> assets = JsonConvert.DeserializeObject<List<TAsset>>(sr.ReadToEnd());

			int i = 0;
			foreach (TAsset asset in assets)
			{
				TAssetRowControlHandler assetRowControlHandler = (TAssetRowControlHandler)Activator.CreateInstance(typeof(TAssetRowControlHandler), asset, i++ % 2 == 0);
				AssetRowEntries.Add(new AssetRowEntry<TAsset, TAssetRowControl, TAssetRowControlHandler>(assetRowControlHandler, true));
			}

			ChunkInfo chunkInfo = ChunkInfo.All.FirstOrDefault(c => c.AssetType == typeof(TAsset));
			FilterHighlightColor = chunkInfo.GetColor() * 0.25f;
		}

		public abstract void UpdateGui(TAsset asset);

		public void UpdateTagHighlighting()
		{
			foreach (AssetRowEntry<TAsset, TAssetRowControl, TAssetRowControlHandler> assetRowEntry in AssetRowEntries.Where(a => a.IsActive))
				assetRowEntry.AssetRowControlHandler.UpdateTagHighlighting(CheckedFilters, FilterHighlightColor);
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
				TAsset asset = AssetRowEntries.Where(a => a.AssetRowControlHandler.Asset.AssetName == Path.GetFileNameWithoutExtension(filePath).Replace("_fragment", "").Replace("_vertex", "")).Cast<TAsset>().FirstOrDefault();
				if (asset != null)
				{
					asset.EditorPath = filePath.Replace("_fragment", "").Replace("_vertex", "");
					UpdateGui(asset);
				}
			}
		}

		public bool IsComplete()
		{
			foreach (TAsset asset in AssetRowEntries.Select(a => a.AssetRowControlHandler.Asset))
				if (!File.Exists(asset.EditorPath.Replace(".glsl", "_vertex.glsl")))
					return false;
			return true;
		}

		public void CreateFiltersGui()
		{
			IEnumerable<string> tags = AssetRowEntries.Select(a => a.AssetRowControlHandler.Asset).SelectMany(a => a.Tags ?? (new string[] { })).Where(t => !string.IsNullOrEmpty(t)).Distinct().OrderBy(s => s);
			int filterColumnCount = 9;
			int i = 0;
			for (; i < filterColumnCount; i++)
			{
				StackPanel stackPanel = new StackPanel { Tag = i };
				Grid.SetColumn(stackPanel, i);
				filterStackPanels.Add(stackPanel);
			}

			i = 0;
			foreach (string tag in tags)
			{
				int pos = (int)(i++ / (float)tags.Count() * filterColumnCount);
				CheckBox checkBox = new CheckBox { Content = tag };
				filterCheckBoxes.Add(checkBox);
				filterStackPanels.FirstOrDefault(s => (int)s.Tag == pos).Children.Add(checkBox);
			}
		}

		public void ApplyFilter(FilterOperation filterOperation, Dictionary<TAssetRowControl, TextBlock> textBlocks)
		{
			foreach (AssetRowEntry<TAsset, TAssetRowControl, TAssetRowControlHandler> assetRowEntry in AssetRowEntries)
			{
				TextBlock textBlockTags = textBlocks.FirstOrDefault(kvp => kvp.Key == assetRowEntry.AssetRowControlHandler.AssetRowControl).Value;

				if (CheckedFilters.Count() == 0)
				{
					assetRowEntry.IsActive = true;
				}
				else
				{
					assetRowEntry.IsActive = filterOperation switch
					{
						FilterOperation.And => CheckedFilters.All(t => assetRowEntry.AssetRowControlHandler.Asset.Tags.Contains(t)),
						FilterOperation.Or => assetRowEntry.AssetRowControlHandler.Asset.Tags.Any(t => CheckedFilters.Contains(t)),
						_ => throw new NotImplementedException($"{nameof(FilterOperation)} {filterOperation} not implemented in {nameof(ApplyFilter)} method.")
					};
				}
			}

			UpdateTagHighlighting();
		}

		public List<AssetRowEntry<TAsset, TAssetRowControl, TAssetRowControlHandler>> ApplySort()
		{
			IEnumerable<AssetRowEntry<TAsset, TAssetRowControl, TAssetRowControlHandler>> query = AssetRowEntries.Where(a => a.IsActive);
			query = ActiveSorting.IsAscending ? query.OrderBy(ActiveSorting.SortingFunction) : query.OrderByDescending(ActiveSorting.SortingFunction);
			return query.ToList();
		}
	}
}