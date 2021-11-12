# SamplePlugin
Simple example plugin for XIVLauncher/Dalamud, that shows both a working plugin and an associated UI test project, to allow for building and tweaking the UI without having to run the game.

This is not designed to be the simplest possible example, but neither is it designed to cover everything you might want to do.

I'm mostly hoping this helps some people to build out their UIs without having to constantly jump in and out of game.

**Sample Plugin is structured as Visual Studio Solution/Projects and will need to be adapted by anyone wishing to use something else.**

### Main Points
* The intention is less that any of this is used directly in other projects, and more to show how similar things can be done.
* Hides data files from visual studio to reduce clutter
  * Also allows having data files in different paths than VS would usually allow if done in the IDE directly

#### SamplePlugin
* Simple functional plugin
  * Slash command
  * Main UI
  * Settings UI
  * Image loading
  * Plugin json
* Simple, slightly-improved plugin configuration handling
* Copies all necessary plugin files to the output directory
  * Does not copy dependencies that are provided by dalamud
  * Output directory can be zipped directly and have exactly what is required

By default, building should out directly into the XIVLauncher's DevPlugins and be automatically loaded upon game launch.

#### UIDev
* Basic ImGui testbed application project
  * Allows testing UI changes without needing to run the game
  * UI environment provided should match what is seen in game
  * Defaults to an invisible fullscreen overlay; can easily be changed to use an opaque window etc
  * Currently relies more on copy/paste of your UI code than fully generic objects (though this could be done)

Can be built as-us and run stand-alone directly resulting in a "pop-up" of the UI.
  
### First time setup

If you have not already done so, install the [latest version of the XIVLauncher](https://github.com/goatcorp/FFXIVQuickLauncher).
* The dalamud binaries come bundled with it and will get setup automatically and is the **highly preferred

You may need to fixup the library dependencies (for both projects), to point at your local dalamud binary directory.

Both projects are preconfigured to target the current live release's binaries in `%APPDATA%\XivLauncher\addon\Hooks\`.  That said, updates, non-standard launcher installations, or, custom dalamud builds may result in the binaries being located elsewhere.

The simplest way to do this is to find this section in the .csproj files:
```
  <PropertyGroup>
    <DalamudLibPath>$(appdata)\XIVLauncher\addon\Hooks\dev\</DalamudLibPath>
  </PropertyGroup>
```
and simply update the path as required.
  
Once you've got it working, clear out what you don't need in UITest.cs and implement your own local UI under Draw()
  
