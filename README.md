> ⚠️ **Don't click Fork!**
> 
> This is a GitHub Template repo. If you want to use this for a plugin, just [use this template][new-repo] to make a new repo!
>
> ![image](https://github.com/goatcorp/SamplePlugin/assets/16760685/d9732094-e1ed-4769-a70b-58ed2b92580c)

# SamplePlugin

[![Use This Template badge](https://img.shields.io/badge/Use%20This%20Template-0?logo=github&labelColor=grey)][new-repo]


Simple example plugin for Dalamud.

This is not designed to be the simplest possible example, but it is also not designed to cover everything you might want to do. For more detailed questions, come ask in [the Discord](https://discord.gg/holdshift).

## Main Points

* Simple functional plugin
  * Slash command
  * Main UI
  * Settings UI
  * Image loading
  * Plugin json
* Simple, slightly-improved plugin configuration handling
* Project organization
  * Copies all necessary plugin files to the output directory
    * Does not copy dependencies that are provided by dalamud
    * Output directory can be zipped directly and have exactly what is required
  * Hides data files from visual studio to reduce clutter
    * Also allows having data files in different paths than VS would usually allow if done in the IDE directly


The intention is less that any of this is used directly in other projects, and more to show how similar things can be done.

## How To Use

### Getting Started

To begin, [clone this template repository][new-repo] to your own GitHub account. This will automatically bring in everything you need to get a jumpstart on development. You do not need to fork this repository unless you intend to contribute modifications to it.

Be sure to also check out the [Dalamud Developer Docs][dalamud-docs] for helpful information about building your own plugin. The Developer Docs includes helpful information about all sorts of things, including [how to submit][submit] your newly-created plugin to the official repository. Assuming you use this template repository, the provided project build configuration and license are already chosen to make everything a breeze.

[new-repo]: https://github.com/new?template_name=SamplePlugin&template_owner=goatcorp
[dalamud-docs]: https://dalamud.dev
[submit]: https://dalamud.dev/plugin-development/plugin-submission

### Prerequisites

SamplePlugin assumes all the following prerequisites are met:

* XIVLauncher, FINAL FANTASY XIV, and Dalamud have all been installed and the game has been run with Dalamud at least once.
* XIVLauncher is installed to its default directories and configurations.
  * If a custom path is required for Dalamud's dev directory, it must be set with the `DALAMUD_HOME` environment variable.
* A .NET Core 8 SDK has been installed and configured, or is otherwise available. (In most cases, the IDE will take care of this.)

### Building

1. Open up `SamplePlugin.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `SamplePlugin/bin/x64/Debug/SamplePlugin.dll` (or `Release` if appropriate.)

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    * In here, go to `Experimental`, and add the full path to the `SamplePlugin.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * In here, go to `Dev Tools > Installed Dev Plugins`, and the `SamplePlugin` should be visible. Enable it.
3. You should now be able to use `/pmycommand` (chat) or `pmycommand` (console)!

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load your plugin on startup through the Plugin Installer.

### Reconfiguring for your own uses

Basically, just replace all references to `SamplePlugin` in all of the files and filenames with your desired name, then start building the plugin of your dreams. You'll figure it out 😁

Dalamud will load the JSON file (by default, `SamplePlugin/SamplePlugin.json`) next to your DLL and use it for metadata, including the description for your plugin in the Plugin Installer. Make sure to update this with information relevant to _your_ plugin!
