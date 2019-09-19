# Devil Daggers Asset Editor 0.2.0.0 ([Download](https://devildaggers.info/tools/DevilDaggersAssetEditor/DevilDaggersAssetEditor0.2.0.0.zip))

Devil Daggers Asset Editor is a tool that can be used to extract assets from the Devil Daggers resource files, as well as compressing them back to create mods. The program is currently in beta.

## Main features

~~Strikethrough~~ features are upcoming/planned features. These are not supported yet.

- Extracting asset types
	- Audio
	- Models
	- Model bindings
	- Shaders
	- Textures

- Extracting binary files
	- res/audio
	- res/dd
	- core/core
	- ~~dd/particle~~

- Compressing asset types
	- Audio
	- ~~Models~~
	- ~~Model bindings~~
	- ~~Shaders~~
	- ~~Textures~~
	
- Compressing binary files
	- res/audio
	- ~~res/dd~~
	- ~~core/core~~
	- ~~dd/particle~~

## System requirements

- Microsoft Windows (64-bit)
- .NET Framework 4.6.1

## Installation

1. Download the zip file.
2. Unzip its contents.
3. Run DevilDaggersAssetEditor.exe inside the folder.

## Links

- [DevilDaggers.info website](https://devildaggers.info)
- [Main web page for DevilDaggersAssetEditor](https://devildaggers.info/Tools/DevilDaggersAssetEditor)
- [Discord server](https://discord.gg/NF32j8S)

## Changelog

#### 0.2.0.0 - September 19, 2019

- Loudness is now exported as a .ini file rather than a .wav file.
- Added functionality to export the current loudness values.
- Added ability to save and open "mod" files (".audio" extension for audio mods) which are files containing the asset paths and loudness values. This removes the need to import/export loudness values when closing the application and is way more convenient than having to extract and compress huge files every time as well. Note that the "mod" files only contain local paths, so sharing them will not work.
- Fixed not being able to type decimal values in the loudness fields.
- Added support to automatically check for new versions of the program.
- Added icon.
- Added About window and other menu items.
- Added logging.
- Added description for audio assets (WIP).
- Many GUI and code improvements.

#### 0.1.0.0 - September 11, 2019

- Beta release.

## Credits

This program is heavily based on [Devil Daggers Extractor by pmcc](https://github.com/pmcc/devil-daggers-extractor), and inspired by [Sojk](https://github.com/sojk)'s asset editor created with QT.

## License

MIT License

Copyright (c) 2019 Noah Stolk

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.