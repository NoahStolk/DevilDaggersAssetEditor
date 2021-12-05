namespace OpenAlBindings;

public static class Al
{
	#region AL

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr alGetString(int name);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern sbyte alIsExtensionPresent(string extensionName);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alGetSourcei(uint sourceID, IntSourceProperty property, out int value);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alSourcePlay(uint sourceID);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alSourcePause(uint sourceID);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alSourceStop(uint sourceID);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alSourceQueueBuffers(uint sourceID, int number, uint[] bufferIDs);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alSourceUnqueueBuffers(uint sourceID, int buffers, uint[] buffersDequeued);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alGenSources(int count, uint[] sources);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alDeleteSources(int count, uint[] sources);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alGetSourcef(uint sourceID, FloatSourceProperty property, out float value);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alGetSource3f(uint sourceID, FloatSourceProperty property, out float val1, out float val2, out float val3);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alSourcei(uint sourceID, IntSourceProperty property, int value);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alSourcef(uint sourceID, FloatSourceProperty property, float value);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alSource3f(uint sourceID, FloatSourceProperty property, float val1, float val2, float val3);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alGetBufferi(uint bufferID, AlEnum property, out int value);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alGenBuffers(int count, uint[] bufferIDs);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alBufferData(uint bufferID, AudioFormat format, byte[] data, int byteSize, uint frequency);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alDeleteBuffers(int numBuffers, uint[] bufferIDs);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alListenerf(FloatSourceProperty param, float val);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alListenerfv(FloatSourceProperty param, float[] val);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alListener3f(FloatSourceProperty param, float val1, float val2, float val3);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alGetListener3f(FloatSourceProperty param, out float val1, out float val2, out float val3);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alGetListenerf(FloatSourceProperty param, out float val);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alGetListenerfv(FloatSourceProperty param, float[] val);

	#endregion AL

	#region ALC

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr alcGetString([In] IntPtr device, int name);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern sbyte alcIsExtensionPresent([In] IntPtr device, string extensionName);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alcDestroyContext(IntPtr context);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alcCaptureStart(IntPtr device);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alcCaptureStop(IntPtr device);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alcCaptureSamples(IntPtr device, IntPtr buffer, int numSamples);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr alcCaptureOpenDevice(string deviceName, uint frequency, AudioFormat format, int bufferSize);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alcCaptureCloseDevice(IntPtr device);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alcGetIntegerv(IntPtr device, AlCEnum param, int size, out int data);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr alcOpenDevice(string deviceName);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr alcCloseDevice(IntPtr handle);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr alcCreateContext(IntPtr device, IntPtr attrlist);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void alcMakeContextCurrent(IntPtr context);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr alcGetContextsDevice(IntPtr context);

	[DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr alcGetCurrentContext();

	#endregion ALC
}
