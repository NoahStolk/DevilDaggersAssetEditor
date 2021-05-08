﻿using DevilDaggersAssetEditor.User;
using DevilDaggersAssetEditor.Wpf.Clients;
using DevilDaggersAssetEditor.Wpf.Gui.Windows;
using DevilDaggersAssetEditor.Wpf.Network;
using DevilDaggersAssetEditor.Wpf.Utils;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersCore.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevilDaggersAssetEditor.Wpf.Gui.UserControls
{
	public partial class DownloadModsControl : UserControl
	{
		private const int _pageSize = 40;

		public static readonly RoutedUICommand FirstPageCommand = new("FirstPage", nameof(FirstPageCommand), typeof(DownloadModsControl), new() { new KeyGesture(Key.OemComma, ModifierKeys.Control) });
		public static readonly RoutedUICommand PreviousPageCommand = new("PreviousPage", nameof(PreviousPageCommand), typeof(DownloadModsControl), new() { new KeyGesture(Key.OemComma) });
		public static readonly RoutedUICommand NextPageCommand = new("NextPage", nameof(NextPageCommand), typeof(DownloadModsControl), new() { new KeyGesture(Key.OemPeriod) });
		public static readonly RoutedUICommand LastPageCommand = new("LastPage", nameof(LastPageCommand), typeof(DownloadModsControl), new() { new KeyGesture(Key.OemPeriod, ModifierKeys.Control) });

		private int _pageIndex;
		private int _totalMods;

		private ModSorting _activeModSorting;
		private readonly List<ModSorting> _modSortings = new();

		private readonly List<ModGrid> _modGrids = new();

		private static readonly SolidColorBrush _even = new(Color.FromArgb(31, 127, 127, 127));
		private static readonly SolidColorBrush _odd = new(Color.FromArgb(31, 91, 91, 91));

		private string? _selectedModName;

		public DownloadModsControl()
		{
			InitializeComponent();

			// Set sorting values and GUI header.
			int sortingIndex = 0;
			List<bool> cachedDirections = UserHandler.Instance.Cache.DownloadSortingDirections ?? new();
			List<ModSorting> sortings = new()
			{
				new(sortingIndex, "Name", "Name", GetCachedDirection(sortingIndex++, true), m => m.Name),
				new(sortingIndex, "Author", "Author", GetCachedDirection(sortingIndex++, true), m => string.Join(", ", m.Authors), m => m.Name),
				new(sortingIndex, "Last updated", "Last updated", GetCachedDirection(sortingIndex++, false), m => m.LastUpdated),
				new(sortingIndex, "Asset types", "Asset types", GetCachedDirection(sortingIndex++, false), m => m.AssetModTypes, m => m.Name),
				new(sortingIndex, "Prohibited assets", "Prohibited assets", GetCachedDirection(sortingIndex++, false), m => m.ContainsProhibitedAssets, m => m.Name),
				new(sortingIndex, "File size", "File size", GetCachedDirection(sortingIndex++, false), m => m.ModArchive?.FileSize, m => m.Name),
				new(sortingIndex, "File size (extracted)", "File size (extracted)", GetCachedDirection(sortingIndex++, false), m => m.ModArchive?.FileSizeExtracted, m => m.Name),
			};

			bool GetCachedDirection(int index, bool defaultDirection)
				=> cachedDirections.Count > index ? cachedDirections[index] : defaultDirection;

			int i = 0;
			Uri sortImageUri = ContentUtils.MakeUri(Path.Combine("Content", "Images", "Buttons", "Sort.png"));
			foreach (ModSorting sorting in sortings)
			{
				Button button = new()
				{
					ToolTip = $"Sort by \"{sorting.FullName}\"",
					Width = 18,
					Content = new Image
					{
						Source = new BitmapImage(sortImageUri),
						Stretch = Stretch.None,
					},
				};
				button.Click += (_, _) => SortModsButton_Click(sorting);
				sorting.Button = button;

				_modSortings.Add(sorting);

				StackPanel stackPanel = new()
				{
					Orientation = Orientation.Horizontal,
					HorizontalAlignment = i < 5 ? HorizontalAlignment.Left : HorizontalAlignment.Right,
				};
				stackPanel.Children.Add(new TextBlock
				{
					FontWeight = FontWeights.Bold,
					Text = sorting.DisplayName,
				});
				stackPanel.Children.Add(button);
				Grid.SetColumn(stackPanel, i++);
				ModHeaders.Children.Add(stackPanel);
			}

			int? index = UserHandler.Instance.Cache.DownloadSortingIndex;
			if (!index.HasValue || index < 0 || index > _modSortings.Count - 1)
				index = 2;

			_activeModSorting = _modSortings[index.Value];

			// Set mod GUI grids.
			for (i = 0; i < _pageSize; i++)
			{
				Grid grid = new()
				{
					Background = (i % 2 == 0) ? _even : _odd,
					Margin = new(2, 0, 2, 0),
				};
				List<TextBlock> textBlocks = new();

				for (int j = 0; j < sortings.Count; j++)
				{
					grid.ColumnDefinitions.Add(new() { Width = new GridLength(1, GridUnitType.Star) });

					TextBlock textBlock = new()
					{
						HorizontalAlignment = j < 5 ? HorizontalAlignment.Left : HorizontalAlignment.Right,
						Foreground = ColorUtils.ThemeColors["Text"],
					};
					Grid.SetColumn(textBlock, j);

					textBlocks.Add(textBlock);
					grid.Children.Add(textBlock);
				}

				_modGrids.Add(new(grid, textBlocks));
				ModsListView.Items.Add(grid);
			}

			AuthorSearchTextBox.Text = UserHandler.Instance.Cache.DownloadAuthorFilter ?? string.Empty;
			ModSearchTextBox.Text = UserHandler.Instance.Cache.DownloadModFilter ?? string.Empty;

			UpdateMods();
			UpdatePageLabel();
		}

		public int LastPageIndex => (_totalMods - 1) / _pageSize;

		private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
			=> e.CanExecute = true;

		#region GUI

		private void UpdateMods()
		{
			IEnumerable<Mod> mods = NetworkHandler.Instance.Mods;

			// Sorting
			int sortIndex = 0;
			foreach (Func<Mod, object?> sortingFunction in _activeModSorting.SortingFunctions)
			{
				if (sortIndex == 0)
					mods = _activeModSorting.Ascending ? mods.OrderBy(sortingFunction) : mods.OrderByDescending(sortingFunction);
				else if (mods is IOrderedEnumerable<Mod> orderedMods)
					mods = _activeModSorting.Ascending ? orderedMods.ThenBy(sortingFunction) : orderedMods.ThenByDescending(sortingFunction);
				else
					throw new($"Could not apply sorting because '{nameof(orderedMods)}' was not of type '{nameof(IOrderedEnumerable<Mod>)}'.");
				sortIndex++;
			}

			// Filtering
			if (!string.IsNullOrWhiteSpace(ModSearchTextBox.Text))
				mods = mods.Where(sf => sf.Name.Contains(ModSearchTextBox.Text, StringComparison.InvariantCultureIgnoreCase));
			if (!string.IsNullOrWhiteSpace(AuthorSearchTextBox.Text))
				mods = mods.Where(sf => sf.Authors.Any(s => s.Contains(AuthorSearchTextBox.Text, StringComparison.InvariantCultureIgnoreCase)));

			_totalMods = mods.Count();
			_pageIndex = Math.Min(LastPageIndex, _pageIndex);

			// Paging
			mods = mods.Skip(_pageIndex * _pageSize).Take(_pageSize);

			List<Mod> modsFinal = mods.ToList();
			for (int i = 0; i < _pageSize; i++)
				FillModGrid(i, i < modsFinal.Count ? modsFinal[i] : null);
		}

		private void FillModGrid(int index, Mod? mod)
		{
			ModGrid grid = _modGrids[index];
			grid.Mod = mod;
			if (mod == null)
			{
				grid.TextBlocks[0].Text = string.Empty;
				grid.TextBlocks[1].Text = string.Empty;
				grid.TextBlocks[2].Text = string.Empty;
				grid.TextBlocks[3].Text = string.Empty;
				grid.TextBlocks[4].Text = string.Empty;
				grid.TextBlocks[5].Text = string.Empty;
				grid.TextBlocks[6].Text = string.Empty;
			}
			else
			{
				grid.TextBlocks[0].Text = mod.Name;
				grid.TextBlocks[1].Text = string.Join(", ", mod.Authors);
				grid.TextBlocks[2].Text = mod.LastUpdated == DateTime.MinValue ? "Unknown" : mod.LastUpdated.ToString("dd MMM yyyy");
				grid.TextBlocks[3].Text = mod.AssetModTypes.ToString() ?? "N/A";
				grid.TextBlocks[4].Text = mod.ContainsProhibitedAssets.HasValue ? mod.ContainsProhibitedAssets.Value ? "Yes" : "No" : "Unknown";
				grid.TextBlocks[5].Text = FormatUtils.FormatFileSize(mod.ModArchive?.FileSize ?? 0);
				grid.TextBlocks[6].Text = FormatUtils.FormatFileSize(mod.ModArchive?.FileSizeExtracted ?? 0);
			}
		}

		#endregion GUI

		#region Events

		private void ModsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PreviewBinariesList.Children.Clear();
			PreviewScreenshotsList.Children.Clear();

			Mod? mod = _modGrids[ModsListView.SelectedIndex].Mod;
			_selectedModName = mod?.Name;
			DownloadModButton.IsEnabled = mod?.ModArchive != null;
			PreviewName.Content = _selectedModName ?? "No mod selected";
			PreviewDescription.Text = mod?.HtmlDescription;
			if (mod?.ModArchive != null)
			{
				int i = 0;
				foreach (ModBinary binary in mod.ModArchive.Binaries)
					PreviewBinariesList.Children.Add(new TextBlock { Text = binary.Name, Background = (i++ % 2 == 0) ? _even : _odd, Margin = new Thickness(4, 0, 0, 0) });

				foreach (string screenshotFileName in mod.ScreenshotFileNames)
				{
					PreviewScreenshotsList.Children.Add(new Image
					{
						Margin = new Thickness(4, 0, 0, 0),
						MaxWidth = 256,
						Stretch = Stretch.Fill,
						HorizontalAlignment = HorizontalAlignment.Left,
						Source = new BitmapImage(new Uri($"https://devildaggers.info/mod-screenshots/{mod.Name}/{screenshotFileName}")),
					});
				}
			}
		}

		private async void DownloadModButton_Click(object sender, RoutedEventArgs e)
		{
			if (_selectedModName == null)
				return;

			string modsDirectory = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods");

			ModArchive? archive = NetworkHandler.Instance.Mods.Find(m => m.Name == _selectedModName)?.ModArchive;
			if (archive != null)
			{
				foreach (ModBinary binary in archive.Binaries)
				{
					if (File.Exists(Path.Combine(modsDirectory, binary.Name)))
					{
						ConfirmWindow window = new("File already exists", $"The mod '{_selectedModName}' contains a binary called '{binary.Name}'. A file with the same name already exists in the mods directory. Are you sure you want to overwrite it by downloading the '{_selectedModName}' mod?", false);
						window.ShowDialog();

						if (window.IsConfirmed != true)
							return;
					}
				}
			}

			DownloadAndInstallModWindow downloadingWindow = new();
			downloadingWindow.Show();
			await downloadingWindow.DownloadAndInstall(modsDirectory, _selectedModName);
		}

		private void ReloadButton_Click(object sender, RoutedEventArgs e)
		{
			AuthorSearchTextBox.Text = string.Empty;
			ModSearchTextBox.Text = string.Empty;
			ReloadButton.IsEnabled = false;
			ReloadButton.Content = "Loading...";

			using BackgroundWorker thread = new();
			thread.DoWork += (senderDoWork, eDoWork) =>
			{
				Task modsTask = NetworkHandler.Instance.RetrieveModList();
				modsTask.Wait();
			};
			thread.RunWorkerCompleted += (senderRunWorkerCompleted, eRunWorkerCompleted) =>
			{
				UpdateMods();
				UpdatePageLabel();

				ReloadButton.IsEnabled = true;
				ReloadButton.Content = "Reload";
			};

			thread.RunWorkerAsync();
		}

		private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateMods();
			UpdatePageLabel();
		}

		private void ClearAuthorSearchButton_Click(object sender, RoutedEventArgs e)
			=> AuthorSearchTextBox.Text = string.Empty;

		private void ClearModSearchButton_Click(object sender, RoutedEventArgs e)
			=> ModSearchTextBox.Text = string.Empty;

		private void FilterCheckBox_Changed(object sender, RoutedEventArgs e)
		{
			UpdateMods();
			UpdatePageLabel();
		}

		private void SortModsButton_Click(ModSorting modSorting)
		{
			_activeModSorting = modSorting;
			_activeModSorting.Ascending = !_activeModSorting.Ascending;

			UpdateMods();
		}

		private void FirstPage_Click(object sender, RoutedEventArgs e)
			=> FirstPage();

		private void PreviousPage_Click(object sender, RoutedEventArgs e)
			=> PreviousPage();

		private void NextPage_Click(object sender, RoutedEventArgs e)
			=> NextPage();

		private void LastPage_Click(object sender, RoutedEventArgs e)
			=> LastPage();

		private void FirstPage_Executed(object sender, RoutedEventArgs e)
			=> FirstPage();

		private void PreviousPage_Executed(object sender, RoutedEventArgs e)
			=> PreviousPage();

		private void NextPage_Executed(object sender, RoutedEventArgs e)
			=> NextPage();

		private void LastPage_Executed(object sender, RoutedEventArgs e)
			=> LastPage();

		private void FirstPage()
		{
			_pageIndex = 0;
			UpdateMods();
			UpdatePageLabel();
		}

		private void PreviousPage()
		{
			_pageIndex = Math.Max(0, _pageIndex - 1);
			UpdateMods();
			UpdatePageLabel();
		}

		private void NextPage()
		{
			_pageIndex = Math.Min(LastPageIndex, _pageIndex + 1);
			UpdateMods();
			UpdatePageLabel();
		}

		private void LastPage()
		{
			_pageIndex = LastPageIndex;
			UpdateMods();
			UpdatePageLabel();
		}

		private void UpdatePageLabel()
			=> PageLabel.Content = $"Page {_pageIndex + 1} of {LastPageIndex + 1}\nShowing {_pageIndex * _pageSize + 1} - {Math.Min(_totalMods, (_pageIndex + 1) * _pageSize)} of {_totalMods} results";

		public void SaveCache()
		{
			UserHandler.Instance.Cache.DownloadAuthorFilter = AuthorSearchTextBox.Text;
			UserHandler.Instance.Cache.DownloadModFilter = ModSearchTextBox.Text;
			UserHandler.Instance.Cache.DownloadSortingIndex = _activeModSorting.Index;
			UserHandler.Instance.Cache.DownloadSortingDirections = _modSortings.ConvertAll(s => s.Ascending);
		}

		#endregion Events

		#region Classes

		private class ModSorting
		{
			public ModSorting(int index, string fullName, string displayName, bool ascending, params Func<Mod, object?>[] sortingFunctions)
			{
				Index = index;
				FullName = fullName;
				DisplayName = displayName;
				SortingFunctions = sortingFunctions;

				Ascending = ascending;
			}

			public int Index { get; }
			public string FullName { get; }
			public string DisplayName { get; }
			public Func<Mod, object?>[] SortingFunctions { get; }

			public Button? Button { get; set; }
			public bool Ascending { get; set; }
		}

		private class ModGrid
		{
			public ModGrid(Grid grid, List<TextBlock> textBlocks)
			{
				Grid = grid;
				TextBlocks = textBlocks;
			}

			public Grid Grid { get; }
			public List<TextBlock> TextBlocks { get; }

			public Mod? Mod { get; set; }
		}

		#endregion Classes
	}
}
