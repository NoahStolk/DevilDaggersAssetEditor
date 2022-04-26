# Resource binary

## Files

There are 3 files in Devil Daggers which use this format. These are 'audio', 'dd', and 'core'.

- `devildaggers/res/audio` contains all audio.
- `devildaggers/res/dd` contains all other assets (models, textures, shaders).
- `devildaggers/core/core` contains UI-related shaders.

### The 'audio' file

This binary consists of audio files (.wav) and a "loudness" file. It is the largest binary and makes up for almost 90% of all data.

In addition to audio chunks, the audio file contains a loudness file. The loudness file is a plain text file that lists all "loudness" values for most audio. These values are numbers and represent which audio files have priority when playing, e.g. a sound with loudness 2 will be prioritized over a sound with loudness 1 by the audio engine. Both sounds will still be played, but the one with the higher loudness value will sound more apparent than the other. Loudness does not correspond to volume. In other words, specifying a higher loudness value doesn't necessary make a certain audio asset louder on its own. You could say that audio assets with a higher loudness value effectively make all other audio assets with lower values more quiet.

The loudness specifies the volume for 217 audio assets, 5 of these assets do not exist in the current version of the game (`collectgib1loop`, `collectgib2loop`, `collectgib3loop`, `eyepacify`, `ricochetmagic5`).

51 audio assets have no loudness specified. When no loudness is specified, the game defaults to loudness 1.0 for these assets.

Oddly enough, the binary format treats this file exactly the same as the .wav assets. Devil Daggers Asset Editor treats this file as a .ini file however.

### The 'dd' file

This binary consists of models, textures, shaders, and model bindings. The file contains some garbage data in the TOC buffer (unused entries with type `0x11`).

### The 'core' file

This binary consists of UI-related shaders. It is the smallest binary. The file contains some garbage data in the TOC buffer as well as some more garbage at the end of the file.

## Format

The internal structure of resource binaries consists of 3 parts:

| Name | Size in bytes |
|------|---------------|
| Header buffer | 12 |
| Table of contents (TOC) buffer | variable |
| Chunk data buffer | variable |

### Overview of known values

- Header buffer
	- Format identifiers
	- TOC buffer length
- TOC buffer
	- For every chunk:
		- Chunk type
		- Chunk name
		- Chunk data start offset from beginning of file
		- Chunk data length
		- Unknown
- Chunk data buffer
	- For every audio chunk:
		- .wav data
	- For every model binding chunk:
		- Contents in plain text
	- For every model chunk:
		- Model header
			- Index count
			- Vertex count
			- Unknown
		- Model vertices
		- Model indices
		- Unknown
	- For every shader chunk:
		- Shader header
			- Shader name length
			- Vertex shader buffer length
			- Fragment shader buffer length
		- Shader name
		- Vertex shader
		- Fragment shader
	- For every texture chunk:
		- Texture header
			- Unknown
			- Texture width in pixels
			- Texture height in pixels
			- Texture mipmap count
		- For every pixel:
			- Color A
			- Color B
			- Color G
			- Color R

### Header buffer

Fixed-length buffer of 12 bytes. Contains the length of the TOC buffer, as well as format identifiers.

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `3A68783A` | 32-bit unsigned integer | Format identifier | 980969530 |
| `72673A01` | 32-bit unsigned integer | Format identifier | 20604786 |
| `15340000` | 32-bit unsigned integer | Table of contents (TOC) buffer length | 13333 |

### Table of contents (TOC) buffer

Variable-length buffer that lists all the chunks (assets). Here are the first 3 entries in the TOC buffer for the resource file 'dd':

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `10` | 8-bit unsigned integer | Chunk type | 16 |
| `00` | 8-bit unsigned integer | Empty | 0 |
| `6465627567` | String | Chunk name | `debug` |
| `00` | 8-bit unsigned integer | Null terminator | `\0` |
| `21340000` | 32-bit unsigned integer | Data start offset | 13345 |
| `6E070000` | 32-bit unsigned integer | Data buffer length | 1902 |
| `00000000` | ? | ? | ? |

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `11` | 8-bit unsigned integer | Chunk type | 17 |
| `00` | 8-bit unsigned integer | Empty | 0 |
| `6465627567` | String | Chunk name | `debug` |
| `00` | 8-bit unsigned integer | Null terminator | `\0` |
| `8F3B0000` | 32-bit unsigned integer | Data start offset | 15247 |
| `00000000` | 32-bit unsigned integer | Data buffer length | 0 |
| `7BCC0557` | ? | ? | ? |

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `10` | 8-bit unsigned integer | Chunk type | 16 |
| `00` | 8-bit unsigned integer | Empty | 0 |
| `6465707468` | String | Chunk name | `depth` |
| `00` | 8-bit unsigned integer | Null terminator | `\0` |
| `8F3B0000` | 32-bit unsigned integer | Data start offset | 15247 |
| `AB010000` | 32-bit unsigned integer | Data buffer length | 427 |
| `00000000` | ? | ? | ? |

The first byte represent the chunk type. Here's the list of chunk types: 

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `01` | 8-bit unsigned integer | Model | 1 |
| `02` | 8-bit unsigned integer | Texture | 2 |
| `10` | 8-bit unsigned integer | Shader | 16 |
| `11` | 8-bit unsigned integer | Unused/Empty`*` | 17 |
| `20` | 8-bit unsigned integer | Audio | 32 |
| `80` | 8-bit unsigned integer | Model binding | 128 |

`*` These were probably meant for fragment shader entries in the TOC buffer but now seem to be unused. Shaders are now 1 chunk containing 2 files (a vertex shader and a fragment shader), but it might not have been like this during development of the game.

### Chunk data buffer

Variable-length buffer that contains all chunk data. This data differs per chunk type.

#### Audio chunks

The chunk data for an audio chunk is the exact same as the contents of a 2-channel .wav file with 44.1kHz sample rate in PCM format.

#### Model binding chunks

The chunk data for a model binding chunk is simply plain text. 

#### Model chunks

The chunk data for a model chunk consists of 4 parts:

| Name | Size in bytes |
|------|---------------|
| Model header | 10 |
| Model vertices | 32 x the amount of vertices |
| Model indices | 4 x the amount of indices |
| Unknown | Variable |

##### Model header

Here are the model headers for 'dagger', 'hand', and 'hand2':

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `B4000000` | 32-bit unsigned integer | Index count | 180 |
| `A4000000` | 32-bit unsigned integer | Vertex count | 164 |
| `2000` | 16-bit unsigned integer | ? | 32 (this used to be 288 `2001`) |

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `78030000` | 32-bit unsigned integer | Index count | 888 |
| `B9000000` | 32-bit unsigned integer | Vertex count | 185 |
| `2000` | 16-bit unsigned integer | ? | 32 (this used to be 288 `2001`) |

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `A4040000` | 32-bit unsigned integer | Index count | 1188 |
| `EE000000` | 32-bit unsigned integer | Vertex count | 238 |
| `2000` | 16-bit unsigned integer | ? | 32 (this used to be 288 `2001`) |

##### Model vertex format

The vertices are stored as a list of vertex objects, which are defined as:

- Position
- Texture coordinate
- Normal

Here is an example:

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `9A99193D` | 32-bit floating point | Position X | 0.0375 |
| `00000000` | 32-bit floating point | Position Y | 0 |
| `075F983C` | 32-bit floating point | Position Z | 0.0186 |
| `6E12433E` | 32-bit floating point | Texture Coordinate U | 0.19049999 |
| `653B3FBF` | 32-bit floating point | Texture Coordinate V | -0.74700004 |
| `6F1223BF` | 32-bit floating point | Normal X | -0.637 |
| `492E7F3D` | 32-bit floating point | Normal Y | 0.0623 |
| `1158993E` | 32-bit floating point | Normal Z | 0.29950002 |

##### Model index format

The indices are stored as a list of 32-bit unsigned integers.

#### Shader chunks

The chunk data for a shader chunk consists of 4 parts:

| Name | Size in bytes |
|------|---------------|
| Shader header | 12 |
| Shader name | Variable |
| Vertex shader contents | Variable |
| Fragment shader contents | Variable |

##### Shader header

Here are the shader headers for 'debug', 'depth', and 'particle':

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `05000000` | 32-bit unsigned integer | Shader name length | 5 |
| `FB020000` | 32-bit unsigned integer | Vertex shader buffer length | 763 |
| `62040000` | 32-bit unsigned integer | Fragment shader buffer length | 1122 |

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `05000000` | 32-bit unsigned integer | Shader name length | 5 |
| `EA000000` | 32-bit unsigned integer | Vertex shader buffer length | 234 |
| `B0000000` | 32-bit unsigned integer | Fragment shader buffer length | 176 |

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `08000000` | 32-bit unsigned integer | Shader name length | 5 |
| `E8050000` | 32-bit unsigned integer | Vertex shader buffer length | 1512 |
| `0C090000` | 32-bit unsigned integer | Fragment shader buffer length | 2316 |

The shader name is listed so the format reader understands at which point to start reading the vertex and fragment buffers.

The vertex and fragment shader buffers are stored as plain text (GLSL code).

#### Texture chunks

 The chunk data for a texture chunk consists of 2 parts: 
 
| Name | Size in bytes |
|------|---------------|
| Texture header | 11 |
| Pixel data | 4 x the amount of pixels + mipmaps`*` |

`*` Texture mipmap count can be calculated by performing binary logarithm (`Log2n`) on the smallest dimension of the image, increasing the result by 1, and then rounding it down. For instance for an image with a resolution of 256x64: `Math.Floor(Math.Log(64, 2) + 1)` = 7 mipmaps. Here's some code that demonstrates how to calculate the total buffer length of a texture's final pixel data:

```cs
int totalBufferLength = width * height * 4;

if (width != height)
{
	int lengthMod = totalBufferLength;
	for (int i = 1; i < mipmapCount; i++)
	{
		lengthMod /= 4;
		totalBufferLength += lengthMod;
	}
}
else
{
	int lengthMod = width;
	for (int i = 1; i < mipmapCount; i++)
	{
		lengthMod /= 2;
		totalBufferLength += lengthMod * lengthMod * 4;
	}
}

return totalBufferLength;
```

##### Texture header

Here are the texture headers for 'dagger', 'hand', and 'sorathmask':

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `1140` | 16-bit unsigned integer | Identifier? | 16401 |
| `40000000` | 32-bit unsigned integer | Texture width in pixels | 64 |
| `40000000` | 32-bit unsigned integer | Texture height in pixels | 64 |
| `07` | 32-bit unsigned integer | Texture mipmap count | 7 |

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `1140` | 16-bit unsigned integer | Identifier? | 16401 |
| `00010000` | 32-bit unsigned integer | Texture width in pixels | 256 |
| `00010000` | 32-bit unsigned integer | Texture height in pixels | 256 |
| `09` | 32-bit unsigned integer | Texture mipmap count | 9 |

| Binary (hex) | Data type | Meaning | Value |
|--------------|-----------|---------|-------|
| `1140` | 16-bit unsigned integer | Identifier? | 16401 |
| `F0000000` | 32-bit unsigned integer | Texture width in pixels | 240 |
| `F0000000` | 32-bit unsigned integer | Texture height in pixels | 240 |
| `08` | 32-bit unsigned integer | Texture mipmap count | 8 |

##### Pixel data format

Each pixel contains 4 color components in the order: alpha, blue, green, red. All components are 8-bit unsigned integers (bytes). The stride of a texture can be calculated by multiplying the texture width with 4.
