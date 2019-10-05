using IrrKlang;

namespace DevilDaggersAssetEditor.Code.Previewers
{
	public class AudioPreviewer : AbstractPreviewer
	{
		private readonly ISoundEngine engine = new ISoundEngine();

		public ISound Song { get; private set; }
		public ISoundSource SongData { get; private set; }

		public AudioPreviewer()
		{
		}

		public void SongSet(string filePath, float pitch, bool startPaused)
		{
			if (Song != null)
				Song.Stop();

			SongData = engine.GetSoundSource(filePath);
			Song = engine.Play2D(SongData, true, startPaused, true);

			if (Song != null)
			{
				Song.PlaybackSpeed = pitch;
				Song.PlayPosition = 0;
			}
		}

		public void TogglePlay()
		{
			if (Song != null)
				Song.Paused = !Song.Paused;
		}

		public void SeekDragComplete(uint seekValue)
		{
			if (Song != null)
				Song.PlayPosition = seekValue;
		}

		public void PitchReset()
		{
			if (Song != null)
				Song.PlaybackSpeed = 1;
		}

		public void PitchSet(float pitch)
		{
			if (Song != null)
				Song.PlaybackSpeed = pitch;
		}
	}
}