using DevilDaggersAssetCore;
using DevilDaggersAssetEditor.Code;
using DevilDaggersAssetEditor.Code.AssetTabControlHandlers;
using DevilDaggersAssetEditor.Gui.UserControls.AssetControls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DevilDaggersAssetEditor.Gui.UserControls.AssetTabControls
{
	public partial class AudioAssetTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileTypeProperty = DependencyProperty.Register
		(
			nameof(BinaryFileType),
			typeof(string),
			typeof(AudioAssetTabControl)
		);

		public string BinaryFileType
		{
			get => (string)GetValue(BinaryFileTypeProperty);
			set => SetValue(BinaryFileTypeProperty, value);
		}

		public AudioAssetTabControlHandler Handler { get; private set; }

		public AudioAssetTabControl()
		{
			InitializeComponent();

			DispatcherTimer timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 10) };
			timer.Tick += (sender, e) =>
			{
				if (Previewer.Song == null || Previewer.Song.Paused)
					return;

				if (!Previewer.Dragging)
					Previewer.Seek.Value = Previewer.Song.PlayPosition / (float)Previewer.Song.PlayLength * Previewer.Seek.Maximum;

				Previewer.SeekText.Text = $"{EditorUtils.ToTimeString((int)Previewer.Song.PlayPosition)} / {EditorUtils.ToTimeString((int)Previewer.Song.PlayLength)}";
			};
			timer.Start();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new AudioAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (AudioAssetControl ac in Handler.CreateAssetControls())
				AssetEditor.Items.Add(ac);
		}

		private void AssetEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			AudioAssetControl ac = e.AddedItems[0] as AudioAssetControl;

			Handler.SelectAsset(ac.Handler.Asset);
			Previewer.Initialize(ac.Handler.Asset);
		}
	}
}