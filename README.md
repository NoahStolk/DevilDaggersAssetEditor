# DevilDaggersAssetEditor ([Download](https://devildaggers.info/Api/GetTool?toolName=DevilDaggersAssetEditor))
DevilDaggersAssetEditor is a tool that can be used to extract assets from the Devil Daggers resource files, as well as compressing them back to create mods. It is a .NET WPF application built using Visual Studio 2019.

## Frameworks
- .NET Standard 2.0 (Core library)
- .NET Framework 4.6.1 (GUI)

## Languages
- C# 8.0

## Dependencies
- [NetBase](https://github.com/NoahStolk/NetBase)
- [JsonUtils](https://github.com/NoahStolk/JsonUtils)
- [DevilDaggersCore](https://github.com/NoahStolk/DevilDaggersCore)
- [SyntaxHighlighter](https://github.com/NoahStolk/SyntaxHighlighter)

## Main features
- Extracting and compressing asset types
	- Audio
	- Models
	- Model bindings
	- Particles
	- Shaders
	- Textures

- Extracting and compressing binary files
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

## Running from source
- Make sure all the projects listed under "Dependencies" are properly referenced. The sources for the projects need to be within the same root folder as the source for DevilDaggersAssetEditor itself.

## Links
- [DevilDaggers.info website](https://devildaggers.info)
- [Main web page for DevilDaggersAssetEditor](https://devildaggers.info/Tools/DevilDaggersAssetEditor)
- [List of mods made with DevilDaggersAssetEditor](https://devildaggers.info/Mods)
- [Asset mod guide](https://devildaggers.info/Wiki/AssetGuide)
- [Discord server](https://discord.gg/NF32j8S)

## Credits
This program's core functionality is heavily based on [Devil Daggers Extractor by pmcc](https://github.com/pmcc/devil-daggers-extractor), and inspired by [Sojk](https://github.com/sojk)'s asset editor created with QT.

## License
MIT License

Copyright (c) 2019-2020 Noah Stolk

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