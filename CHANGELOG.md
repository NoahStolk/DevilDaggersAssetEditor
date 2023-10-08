# Changelog

## [1.4.0.0] - 2022-05-01

- API updates to support multiple builds. Windows 7 now has a separate build. More builds for other operating systems will follow in the future (not soon).
- Removed compiling data for models that was necessary for V3.0 but is no longer relevant.
- Prevented crash when downloading mod screenshot fails.
- Prevented crash when the app cannot access certain files in the mods directory.
- Fixed URL encoding for mod screenshots.

## [1.3.6.0] - 2022-04-25

- Added caching for checkbox states in the 'Import assets' window.
- Fixed settings not saving after exiting the app if the settings window wasn't opened during that session.

## [1.3.4.0] - 2022-03-05

- Removed quad support for models.
- Fixed incorrectly interpreting model faces exported from certain software.
- Fixed not trimming duplicate spaces in .obj files.
- Added clearer error messages when parsing .obj files.
- Added additional validation when parsing .obj files.
- Improved parsing .wav files for previewing.

## [1.3.0.0] - 2022-01-09

- The application now runs on .NET 6 which is a lot faster and will be officially supported until late 2024.
- You do not need to install the .NET runtime anymore. You do not need to install anything for the program to work as of this update (unless you're running Windows 7).
- Fixed mod screenshots not showing up since the release of the new website.
- Rewrote the audio previewer to use OpenAL.

## [1.2.0.0] - 2021-11-11

- Added new V3.2 textures.
- Added tool to convert image to GLSL code.
- Fixed crash when toggling file in mod manager when new file name already exists.

## [1.0.0.0] - 2021-05-13

- You can now download and install mods hosted on the website from within the application using the new 'Mod manager' window which can be opened by pressing CTRL I or selecting it from the menu.
- You can manage local binaries in the mods folder now. This includes easily enabling or disabling installed mods with a single button.
- You can enable or disable prohibited assets per binary. This means it is no longer necessary to create separate binaries for prohibited assets, as you can now just enable or disable prohibited assets with a single button.
- You can see which custom assets will be loaded into the game based on the currently installed mods. This also takes alphabetic partial mod loading into account.
- Improved model trimming. Vertex and index data is now parsed and additional processing is implemented to prevent the occurrence of vertex rounding errors that result in marking the underlying binary chunk as non-identical to the original one. This was caused by the fact that compiled model chunks differ very slightly between different people's computers.
- Changed UI for shader assets so you cannot set only 1 out of 2 required shader paths.
- Incomplete shader assets are ignored when creating a binary.
- Performance for reading large binaries has been improved significantly.
- Added new section to 'Extract binaries' window for creating a mod file based on the extracted assets.
- Removed the 'Automatically create mod file when extracting binaries' setting as this is no longer useful.
- Fixed not always re-initializing asset previewer when changing paths for the currently selected asset.
- Fixed setting window title to cached mod path even when the file was not found on launch.
- Fixed not resetting cached mod path after creating new mod.
- Various other small bug fixes and layout improvements.
- Some UI performance improvements.

## [0.65.4.0] - 2021-05-06

- Improved shader trimming.
- Added progress bar and log to 'Trim binary' window.
- Descriptions and tags for assets are now retrieved from the DevilDaggers.info API, so changes to descriptions or tags do not require an update for DDAE anymore. The asset list itself is still stored in the application so everything still works when you are not connected to the internet.
- Improved validating binaries.
- Small layout improvements and fixes.

## [0.60.2.1] - 2021-04-22

- Rebuilt application to fix missing dependency.

## [0.60.2.0] - 2021-04-20

- Added missing 'core' shader asset, which fixes creating 'core' mods.
- When making a binary, the 'core' path is set to the original file, since it does not support partial mods.
- Added 'Loading UI...' to initial MainWindow start up instead of blocking GUI thread which results in the entire window being white.
- Added descriptions and tags to most model bindings.
- Fixed feather assets not being listed as prohibited.
- Minor UI improvements.

## [0.56.5.0] - 2021-04-12

- The application now keeps track of whether or not you have any unsaved changes and will ask you if you want to save the mod before proceeding to overwrite it by opening an existing mod, creating a new one, or closing the application.
- The application window title now displays the name of the currently opened mod.
- Added 'File > Save as' menu item.
- Added cancellation to confirmation windows. Clicking [X] on a confirmation window now cancels the operation properly.
- Implemented functionality to trim binaries. This is particularly useful for binaries that were compiled before Devil Daggers V3.1 came out. Back then, partial mods were not supported. With this tool you can make your old binary file a lot smaller by leaving out all the unchanged assets, which can be done by comparing it against the original binary. The tool can be found under Tools > Trim binary file.
- User settings are now saved as settings.json in the local application data Windows folder (access by pressing Windows R and typing %localappdata%).
- Added keyboard shortcuts for most menu items under 'File'.
- Implemented caching for the 'Make binaries' window. The application will remember the input values after closing the window or the application entirely.
- Added [X] button to Settings window.
- Fixed empty settings or cache files crashing the application.
- Removed functionality to mod the particle file.
- Removed Help window. The Help menu item now navigates to the Asset Editor Guide on the website.
- Improved logo.

## [0.46.0.0] - 2021-02-26

- The application is now compatible with the latest version of Devil Daggers. The new shader assets have been added, and the program will display which assets are prohibited (meaning you cannot submit 1000+ scores if these assets are modified).
- The application now runs on .NET 5.0. You will need to install .NET Desktop Runtime 5.0.x to run this version.
- The application now allows creation of partial mods. These are mods that only specify assets that have been modified. It is no longer required to create a binary file containing all assets. It also allows you to load multiple partial mods simultaneously. You can, for instance, load a partial mod that only changes the texture for Skull I, and then load another mod that changes the texture for the arena tiles.
- Added CheckBox to include sub-directories when importing assets.
- Added 'File > New' menu item to easily clear the list of specified paths and start a new mod.
- Made the previewer and asset lists adjust better to the window size so the application works much better on smaller screen resolutions.
- Fixed incorrect description for the post_lut_leveldown texture.
- Fixed underscores not being displayed correctly in previewer labels.
- Fixed hardcoded "Model Binding name" header row that should just say "Asset name" instead.

## [0.39.11.0] - 2020-11-17

- Rewrote the application and GUI entirely.
- Made the menu more intuitive by adding 'Extract binaries', 'Make binaries', and 'Import folders' windows.
- The mod format has changed. DDAE will no longer accept mod files with the extension .audio, .core, .dd, and .particle. Mod files are now saved under a unified extension named .ddae, which holds all asset data even if your mod does not specify, for example, particle assets. The easiest way to migrate is just to re-import your assets and save it as a .ddae file.
- Added more missing asset descriptions and tags.
- Fixed 'chatterminisquid' audio asset being listed as unused while 'chattersquidsmall' is the one that's unused.
- Improved error messages when parsing .obj files.
- Fixed audio previewer not reloading selected asset when setting new path.
- Many other bug fixes, performance improvements, and styling improvements.

## [0.18.4.0] - 2020-10-21

- The application now uses a custom dark theme. General layout for many components has been improved as well.
- Added GLSL syntax highlighting.
- Fixed shader assets not being saved to mod file on extraction.
- Fixed Windows Explorer folder dialogs opening the directory above the specified folder.
- Fixed audio previewer crashing when attempting to play an empty audio file.

## [0.16.16.0] - 2020-10-07

- Fixed loudness values not being set when creating a mod file after extracting an audio binary.
- Fixed "Access is denied" error when attempting to open mod folder in Windows Explorer.
- Fixed "Download" button in UpdateRecommended window navigating to an outdated URL.
- Fixed CheckingForUpdates window not waiting for the API result properly.
- Improved confirmation windows.
- Improved progress logging.
- Improved error messages regarding extracting and making binaries.
- Added more model descriptions and tags.
- Added missing audio tags.
- Removed functionality to open log file from the menu as this doesn't work well with the current logging mechanism.
- Parsing model vertices now supports scientific notation.
- Better error messages are shown when model face data could not be parsed.
- Note: Dark theme is used for windows that are shared between DevilDaggersSurvivalEditor and DevilDaggersAssetEditor. I plan to fully integrate dark theme into DDAE in a future update.

## [0.16.3.0] - 2020-08-23

- Fixed importing folders not working.

## [0.16.2.0] - 2020-08-20

- Rewrote much of the application.
- Removed dependencies.
- Ported to .NET Core. The application is no longer dependent on .NET Framework and does not require .NET Framework 4.6.1.
- Added loading screen.
- Fixed crash when attempting to view log file while it was being used by another process.
- Improved performance.

## [0.13.22.0] - 2020-06-10

- Fixed bug when extracting particles.
- Added Help window.
- Improved particle previewer.

## [0.13.19.0] - 2020-06-05

- Fixed bug when saving a .dd mod file after extracting.

## [0.13.18.0] - 2020-06-03

- Fixed crash that occurred when attempting to open a non-existing initial directory in an explorer dialog.
- Fixed auto-detect button in settings windows setting the path to an incorrect value.
- Fixed opening mods root folder instead of assets root folder when importing or exporting loudness files.
- Fixed texture dimension limit setting affecting non-model textures. Post lut filters, icons, fonts, and title screens will no longer be downscaled.

## [0.13.14.0] - 2020-06-01

- Removed the term 'compression' from the UI since it is misleading. What used to be called compressing is now called making a binary.
- Added missing audio assets to the editor.
- Fixed bugs related to the audio loudness file.

## [0.13.11.0] - 2020-05-29

- The application is now fully compatible with 3D modeling software (e.g. Blender) -- the implementation of turning models into binary data is hereby complete.
  - Making a 'dd' binary now supports UV mapping which means textures are properly rendered onto models.
  - Making a 'dd' binary now supports models consisting of quads.
- You can now sort assets.
- Implemented tags for assets.
- You can now filter assets by tags using the new buttons in the column headers.
- Added more settings.
  - You can now choose whether or not to make use of the 3 standard folders.
  - You can now choose to automatically create a mod file when extracting assets from a binary file.
  - You can now choose to automatically open the folder after extracting assets.
  - There is a new setting that enables automatic downscaling for large textures. This setting is set to 512 by default, since the largest original textures in Devil Daggers are 512x512 pixels. This currently applies to all textures, including post lut filters, icons, fonts, and title screens. This means the entire game will look different when you set this setting to 64 for example. All textures will shrink until they fit in a 64x64 pixel space (so a 256x16 texture will be downscaled to 64x4).
- Added binary file analyzer tool that can be used to visualize the contents of a Devil Daggers binary file. This can be helpful in case your file happens to be unnecessarily large and you want to know what causes it.
- Implemented user caching. This means the application will remember certain values when it shuts down.
  - It will remember which mod files were last opened in the previous session.
  - It will remember which tab was active in the previous session.
  - It will remember the window size from the previous session including whether the application was run in full screen or not.
  - It will remember whether or not 'Auto-play' was enabled in the audio previewer.
- Added vertex and index counts to model previewer.
- Added more descriptions and asset information.
- Fixes
  - Fixed inner window not sizing correctly in full screen.
  - Fixed text overflowing (still WIP, hoping to improve this in a future update).
  - Fixed misleading editor paths for shaders by displaying editor paths for both files.
  - Fixed marking negative loudness values as valid.
  - Fixed showing paths for non-existing files.
  - Fixed texture previewer locking image files even when they're no longer being displayed.
  - Fixed texture previewer locking the currently displayed image file.
  - Fixed not clearing previewers when selecting an empty asset.
  - Fixed crash that occurred when attempting to preview a non-existing file.
- Many layout improvements

## [0.7.14.0] - 2020-03-13

- Fixed crash that occurred when turning textures into binary data.
- Fixed textures not being correctly imported from mod files.

## [0.7.13.0] - 2020-02-15

- Added almost all texture descriptions.

## [0.7.12.0] - 2019-11-30

- Improved model extraction to be more compatible with 3D modeling software.
- Fixed crash that occurred when attempting to turn a model with more texture coordinates or more vertex normals than geometric vertices into binary data.
- Saving mod files now produces indented JSON.

## [0.7.8.0] - 2019-11-26

- Improved model parsing so turning .obj files exported from 3D modeling software into binary data is supported.
- Fixed audio description for daggerseek.
- Added tooltips for audio preview buttons.

## [0.7.5.1] - 2019-11-18

- Updated source code URL.

## [0.7.5.0] - 2019-11-03

- Implemented making shader, model (limited), and texture binaries. Features such as making binaries and using mod files for "dd" and "core" are now available and functional. Note that the editor path for shaders can be misleading. For instance when it says the path is "..boid.glsl", it actually points to the files "..boid_fragment.glsl" and "..boid_vertex.glsl". Also note that .obj parsing (model format) is still limited and might not work depending on what kind of .obj file you're trying to turn into binary data. It does work for the original models, or files in an identical .obj format, so you can successfully swap the original models.
- Implemented audio player with the ability to pitch shift audio real-time.
- Implemented texture, shader, model binding, and particle previewers. A model previewer has not yet been implemented.
- Added Changelog window.
- Fixed the Update Recommended window not closing after clicking the download button.
- Small changes related to maintenance of the website, as well as code refactoring and improvements.
- Removed functionality for compatibility with version 0.2.0.0.
- Many bug fixes, layout improvements, and stability improvements.

## [0.3.3.0] - 2019-09-23

- Created progress bar for lengthy tasks such as making and extracting binaries.
- Many bug fixes.
- Improved audio descriptions.

## [0.3.0.0] - 2019-09-21

- Implemented extracting and making particle binaries, as well as mod files for particle mods. The extracted particle files are in binary (.bin), so there is not much to mod until I figure out what the bytes mean, but you can switch particles and replace them with others.
- Implemented functionality to specify whether or not to use relative paths in mod files.
- Added user settings to specify Devil Daggers root folder, mods root folder, and assets root folder.
- Improve initial directories for file and folder dialogs.
- Small bug and crash fixes.
- The mod file format has been changed, and mod files created using version 0.2.0.0 will no longer open. You'll need to convert these by navigating to Compatibility > Convert 0.2.0.0 mod file format and select your mod file.

## [0.2.0.0] - 2019-09-19

- Loudness is now exported as a .ini file rather than a .wav file.
- Added functionality to export the current loudness values.
- Added ability to save and open "mod" files (".audio" extension for audio mods) which are files containing the asset paths and loudness values. This removes the need to import/export loudness values when closing the application and is way more convenient than having to extract and make huge binary files every time as well. Note that the "mod" files only contain local paths, so sharing them will not work.
- Fixed not being able to type decimal values in the loudness fields.
- Added support to automatically check for new versions of the program.
- Added icon.
- Added About window and other menu items.
- Added logging.
- Added description for audio assets (WIP).
- Many GUI and code improvements.

## [0.1.0.0] - 2019-09-11

- Initial release.
