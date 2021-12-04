using OpenAlBindings;

namespace DevilDaggersAssetEditor.Wpf.Audio;

public class PlaybackDevice
{
	private nint _device;

	public PlaybackDevice(string deviceName)
	{
		DeviceName = deviceName;
	}

	public string DeviceName { get; }
	public nint Context { get; private set; }

	public void MakeCurrent()
	{
		_device = Al.alcOpenDevice(DeviceName);
		Context = Al.alcCreateContext(_device, (nint)0);
		Al.alcMakeContextCurrent(Context);
	}

	public void Delete()
	{
		if (_device == 0)
			return;

		Al.alcDestroyContext(Context);
		Al.alcCloseDevice(_device);
		_device = 0;
	}
}
