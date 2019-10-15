using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using System;
using DevilDaggersAssetEditor.Code.AssetTabControlHandlers;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using DevilDaggersAssetEditor.Code;
using System.IO;

namespace DevilDaggersAssetEditor.GUI.UserControls.AssetTabControls
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

		private bool dragging;

		public AudioAssetTabControlHandler Handler { get; private set; }

		public AudioAssetTabControl()
		{
			InitializeComponent();
			ToggleImage.Source = ((Image)Resources["PlayImage"]).Source;
			ResetPitchImage.Source = ((Image)Resources["ResetPitchImage"]).Source;

			DispatcherTimer timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 10) };
			timer.Tick += (sender, e) =>
			{
				if (Handler.Previewer.Song == null || Handler.Previewer.Song.Paused)
					return;

				if (!dragging)
					Seek.Value = Handler.Previewer.Song.PlayPosition / (float)Handler.Previewer.Song.PlayLength * Seek.Maximum;

				SeekText.Text = $"{EditorUtils.ToTimeString((int)Handler.Previewer.Song.PlayPosition)} / {EditorUtils.ToTimeString((int)Handler.Previewer.Song.PlayLength)}";
			};
			timer.Start();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= UserControl_Loaded;

			Handler = new AudioAssetTabControlHandler((BinaryFileType)Enum.Parse(typeof(BinaryFileType), BinaryFileType));

			foreach (AudioAssetControl ac in Handler.CreateAssetControls())
			{
				AssetEditor.Children.Add(ac);
				ac.MouseDoubleClick += (senderAC, eAC) =>
				{
					Handler.SelectAsset(ac.Handler.Asset);

					AudioName.Text = Handler.SelectedAsset.AssetName;
					DefaultLoudness.Text = Handler.SelectedAsset.PresentInDefaultLoudness ? Handler.SelectedAsset.DefaultLoudness.ToString() : "N/A (Defaults to 1)";
					AudioFile.Text = Handler.SelectedAsset.EditorPath.IsPathValid() ? Path.GetFileName(Handler.SelectedAsset.EditorPath) : Handler.SelectedAsset.EditorPath;

					bool startPaused = !Autoplay.IsChecked ?? true;

					Handler.Previewer.SongSet(Handler.SelectedAsset.EditorPath, (float)Pitch.Value, startPaused);

					if (Handler.Previewer.Song != null)
					{
						ToggleImage.Source = ((Image)Resources[startPaused ? "PlayImage" : "PauseImage"]).Source;

						Seek.Maximum = Handler.Previewer.Song.PlayLength;
						Seek.Value = 0;

						SeekText.Text = $"{EditorUtils.ToTimeString((int)Handler.Previewer.Song.PlayPosition)} / {EditorUtils.ToTimeString((int)Handler.Previewer.Song.PlayLength)}";
						PitchText.Text = $"x {Handler.Previewer.Song.PlaybackSpeed:0.00}";
					}
				};
			}
		}

		private void Toggle_Click(object sender, RoutedEventArgs e)
		{
			if (Handler.Previewer.Song != null)
				ToggleImage.Source = ((Image)Resources[Handler.Previewer.Song.Paused ? "PlayImage" : "PauseImage"]).Source;

			Handler.Previewer.TogglePlay();
		}

		private void Pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!IsInitialized)
				return;

			float pitch = (float)e.NewValue;

			PitchText.Text = $"x {pitch:0.00}";

			Handler.Previewer.PitchSet(pitch);
		}

		private void ResetPitch_Click(object sender, RoutedEventArgs e)
		{
			PitchText.Text = "x 1.00";
			Pitch.Value = 1;

			Handler.Previewer.PitchReset();
		}

		private void Seek_DragStarted(object sender, DragStartedEventArgs e)
		{
			dragging = true;
		}

		private void Seek_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			dragging = false;

			Handler.Previewer.SeekDragComplete((uint)Seek.Value);
		}
	}
}