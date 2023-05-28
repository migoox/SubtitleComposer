# Subtitle Composer
Subtitle Composer is an application designed for creating and editing subtitles for movies, as well as their translations, developed in with C# and WPF. The application allows you to load a video file and create synchronized subtitles for it. Additionally, it supports creating custom plugins with the ".dll" extension by implementing the following interface in Class Library project:
```cs
public interface ISubtitlesPlugin
{
    string Name { get; }
    string Extension { get; }
    ICollection<Subtitle> Load(string path);
    void Save(string path, ICollection<Subtitle> subtitles);
    void SaveTranslation(string path, ICollection<Subtitle> subtitles);
}
```
Compiled .dll file should be placed in `plugins` folder.

<img src="https://github.com/migoox/SubtitleComposer/assets/56317134/aa055c43-06cd-4ebd-a009-d195cd2e5b7d" width=600 />

## Features

The Subtitle Composer application includes the following features:

**Video Player (2 points)**
- The "Open" menu item allows selecting and opening a video file.
- Scrolling the mouse wheel over the video player controls the video volume.
- Clicking on the video player toggles between play and pause.
- Selecting a subtitle from the subtitle table moves the playback to the time when the subtitle is displayed.

**Video Player Controls** 
- "Play," "Pause," and "Stop" buttons perform their respective actions, utilizing the provided graphics: "play.png," "pause.png," "stop.png."
- Text displaying the current playback position in the format "hh:mm:ss.ff."
- Slider showing the current playback position, allowing the user to change it.
- Slider showing the current volume, allowing the user to adjust it.

**Video Player Subtitles**
- Subtitles are displayed above the video player in the bottom center.
- The subtitle text is centered.
- The subtitles are displayed in white color.
- The background of the subtitles is black with 50% transparency.
- The padding of the subtitles is set to 5px.
- At a given time "t," all subtitles that should appear before time "t" and disappear after time "t" are displayed.
- If multiple subtitles should be shown at the same time, they are displayed one below the other in the order of their appearance.

**Subtitle Table - Context Menu**
- "Add" option: Adds a subtitle with the maximum hiding time as the show and hide time.
- "Add after" option: Adds a subtitle with the maximum hiding time among the currently selected subtitles as the show and hide time.
- "Delete" option: Removes the selected subtitles.

**Plugin System - Subtitles Loading/Saving**
- The plugin system allows adding support for loading and saving subtitles in different formats without recompiling the application.
- After loading a plugin, the "Subtitles" menu shows options for loading/saving subtitles or their translations in the appropriate format.
- A plugin that adds support for loading/saving subtitles in the ".srt" (SubRip) format.
- Plugins should be implemented as separate Class Library projects and compiled into DLL files.
