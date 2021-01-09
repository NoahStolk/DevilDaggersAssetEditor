[![Build Status](https://travis-ci.org/NoahStolk/DevilDaggersAssetEditor.svg?branch=master)](https://travis-ci.org/NoahStolk/DevilDaggersAssetEditor)
[![Scc Count Badge](https://sloc.xyz/github/NoahStolk/DevilDaggersAssetEditor/)](https://github.com/NoahStolk/DevilDaggersAssetEditor/)

# DevilDaggersAssetEditor ([Download](https://devildaggers.info/api/tools/DevilDaggersAssetEditor/file))
DevilDaggersAssetEditor is a tool that can be used to extract assets from the Devil Daggers resource files, as well as inserting them back into binaries to create mods.

## Framework
- .NET 5.0

## Language
- C# 9.0

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
- [.NET 5.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/5.0)

## Installation
1. Download the zip file.
2. Unzip its contents.
3. Run DevilDaggersAssetEditor.exe inside the folder.

## Running from source
- Make sure to set the project platform to x64 before building the project.

## Links
- [DevilDaggers.info website](https://devildaggers.info)
- [Main web page for DevilDaggersAssetEditor](https://devildaggers.info/Tools/DevilDaggersAssetEditor)
- [List of mods made with DevilDaggersAssetEditor](https://devildaggers.info/Mods)
- [Asset mod guide](https://devildaggers.info/Wiki/AssetGuide)
- [Discord server](https://discord.gg/NF32j8S)

## Credits
The program's asset extraction functionality is based on [Devil Daggers Extractor by pmcc](https://github.com/pmcc/devil-daggers-extractor) written in C++.

## License
MIT License

Copyright (c) 2019-2021 Noah Stolk

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
