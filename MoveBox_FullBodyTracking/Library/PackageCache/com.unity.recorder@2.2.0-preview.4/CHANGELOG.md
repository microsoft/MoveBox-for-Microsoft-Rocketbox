# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).
## [2.2.0-preview.4] - 2020-04-08
### Public API and bugfixes
- Added public api's to allow loading previously saved recorder lists.
- Fixed bug where the first few frames after going in play-mode were not recorded. 
- Fixed a bug where the texture sampling recorder that did not correctly support the physical cameras. 
- Fixed an issue when starting a record session from scripts.
- Fixed movie recoder's default framerate value being not set in the API. Default value is now set at 30 FPS.

## [2.1.0-preview.1] - 2019-12-17
### Public API and HDR recording
- Added public APIs to be able to implement custom recorders.
- Added the ability to enable recording HDR images (available only with HDRP)

## [2.0.3-preview.1] - 2019-08-13
### 2019.3, HDRP and build Fixes
- iOS Cloud build fails with an `exportArchive: Code signing "fccore.bundle" failed` error
- Recorder Clip with a Targeted Camera input gives black images with HDRP
- In 2019.3, HDRP is out of experimental and namespace was renamed
- Recorder settings warnings were not displayed in the console at recording start
- Massive documentation update
- Fixed "NullReferenceException" errors related to changes in GameView in 2019.3.0b1.

## [2.0.2-preview.2] - 2019-05-28
### Fixes and Linux support
- Add Gif and Legacy Recorders core library binaries for Linux
- Fix build errors related to fccore
- Fix a bug with Recorder Clip that would produce one frame movie if never displayed in the inspector
- Warning clean-up in 2018.4
- Remove ActiveCamera target when HDRP is available
- Fixed "NullReferenceException" errors related to changes in GameView in 2019.3

## [2.0.1-preview.1] - 2019-05-20
### Audio Recorder and some bug fixes
- Integrate AudioRecorder in the package
- Fixed Recorder labels that were not editable anymore
- Fixed Recorder List presets that were reloaded with an empty name
- Clean UIElements warning in 2019.2

## [2.0.0-preview.6] - 2019-03-26
### First package release of the *Unity Recorder*.
This mainly address moving from Asset Store to Package Manager. It also includes :
- Timeline dependency fix : since 2019.1 Timeline is a package. Code changes are compatible with
both 2018.3 (2018.3.4f1 and up) and 2019.1 (2019.1.0b2 and up).
- Updates to use official UIElements module as experimental API is deprecated in 2019.1.
- Warnings clean-up
- Samples fixes : documentation updates and proper asmdef to avoid issues during build
- Improved texture readback. Most speed improvements will be effective with previous versions,
 BUT at their top in 2019.1.
- Ability to capture a Light Weight Render Pipeline camera (requires Scriptable Render Pipeline > 5.3.0)

## [1.0.2] - 2018-09-07
### Custom resolutions. Multi-Scene editing. Various bug fixes.
- Ability to use custom resolution and custom aspect ratio
- Fixed GameObject reference sometimes resetting to None when in multi-scenes editing
- Fixed the Recorder Clip duplication issue
- Little improvement for errors reporting in the Recorder Window
- Fixed 360 View being too dark when in linear color space
- Fixed Flip Vertical being too dark in linear color space
- Fixed frame skipped issue when two or more recorders end at the same frame.
- Ability to change/reset the Take value for all recorders
- Added option to exit Playmode automatically when recording's stopped

## [1.0.1] - 2018-08-17
### 2018.1 support. Various UX fixes.
- Added support for 2018.1 (2018.1.9f1 and up)
- UI Fixes when reducing RecorderWindow size in 2018.2 and up
- Added icons for messages in the Recorder Status Bar
- Ability to use Arrow keys to switch between recorders
- Added visual indication for when the Recorder List has focus

## [1.0.0] - 2018-08-02
### First release of the *Unity Recorder*.
This is mainly a UX revamp of the Asset Store's Media Recorder. Main improvement is the ability to have multiple recorders in parallel.
