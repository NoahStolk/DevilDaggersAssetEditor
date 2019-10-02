# Devil Daggers Asset Editor 0.3.3.0 ([Download](https://devildaggers.info/tools/DevilDaggersAssetEditor/DevilDaggersAssetEditor0.3.3.0.zip))

Devil Daggers Asset Editor is a tool that can be used to extract assets from the Devil Daggers resource files, as well as compressing them back to create mods. The program is currently in beta.

## Main features

- Extracting asset types
	- Audio
	- Models
	- Model bindings
	- Particles (binary)
	- Shaders
	- Textures

- Extracting binary files
	- res/audio
	- res/dd
	- core/core
	- dd/particle

- Compressing asset types
	- Audio
	- Models
	- Model bindings
	- Particles
	- Shaders
	- Textures
	
- Compressing binary files
	- res/audio
	- res/dd
	- core/core
	- dd/particle

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

#### 0.x.x.x - To Be Released

- Implemented shader, model, and texture compression. Features such as compressing and using mod files for "dd" and "core" are now available and functional.

#### 0.3.3.0 - September 23, 2019

- Created progress bar for lengthy tasks such as compressing and extracting.
- Many bug fixes.
- Improved audio descriptions.

#### 0.3.0.0 - September 19, 2019

- Implemented particle extraction, compression, and mod files. The particle files are in binary, so there is not much to mod until I figure out what the bytes mean, but you can switch particles and replace them with others.
- Implemented functionality to specify whether or not to use relative paths in mod files.
- Added user settings to specify Devil Daggers root folder, mods root folder, and assets root folder.
- Improve initial directories for file and folder dialogs.
- Small bug and crash fixes.

Note that the mod file format has been changed, and mod files created using version 0.2.0.0 will no longer open. You'll need to convert these by navigating to Compatibility > Convert 0.2.0.0 mod file format and select your mod file.

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

- Initial beta release.

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