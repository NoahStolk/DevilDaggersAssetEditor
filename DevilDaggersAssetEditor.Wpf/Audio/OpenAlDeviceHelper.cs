using OpenAlBindings;
using OpenAlBindings.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DevilDaggersAssetEditor.Wpf.Audio;

public static class OpenAlDeviceHelper
{
	private static PlaybackDevice[]? _playbackDevices;

	public static PlaybackDevice[] PlaybackDevices
	{
		get
		{
			if (_playbackDevices == null)
			{
				string[] strings = Array.Empty<string>();
				if (GetIsExtensionPresent("ALC_ENUMERATE_ALL_EXT"))
					strings = ReadStringsFromMemory(Al.alcGetString((nint)0, (int)AlCStrings.ALC_ALL_DEVICES_SPECIFIER));
				else if (GetIsExtensionPresent("ALC_ENUMERATION_EXT"))
					strings = ReadStringsFromMemory(Al.alcGetString((nint)0, (int)AlCStrings.ALC_DEVICE_SPECIFIER));

				_playbackDevices = new PlaybackDevice[strings.Length];
				for (int i = 0; i < _playbackDevices.Length; i++)
					_playbackDevices[i] = new PlaybackDevice(strings[i]);
			}

			return _playbackDevices;
		}
	}

	private static string[] ReadStringsFromMemory(nint location)
	{
		List<string> strings = new();

		bool lastNull = false;
		int i = -1;
		byte c;
		while (!((c = Marshal.ReadByte(location, ++i)) == '\0' && lastNull))
		{
			if (c == '\0')
			{
				lastNull = true;

				strings.Add(Marshal.PtrToStringAnsi(location, i));
				location += i + 1;
				i = -1;
			}
			else
			{
				lastNull = false;
			}
		}

		return strings.ToArray();
	}

	private static bool GetIsExtensionPresent(string extension)
	{
		sbyte result = extension.StartsWith("ALC") ? Al.alcIsExtensionPresent((nint)0, extension) : Al.alIsExtensionPresent(extension);
		return result == 1;
	}
}
