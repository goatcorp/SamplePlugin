using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Lumina.Excel.Sheets;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly string goatImagePath;
    private readonly Plugin plugin;

    // We give this window a hidden ID using ##.
    // The user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, string goatImagePath)
        : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.goatImagePath = goatImagePath;
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings"))
        {
            plugin.ToggleConfigUi();
        }

        ImGui.Spacing();

        // Normally a BeginChild() would have to be followed by an unconditional EndChild(),
        // ImRaii takes care of this after the scope ends.
        // This works for all ImGui functions that require specific handling, examples are BeginTable() or Indent().
        using (var child = ImRaii.Child("SomeChildWithAScrollbar", Vector2.Zero, true))
        {
            // Check if this child is drawing
            if (child.Success)
            {
                ImGui.Text("Have a goat:");
                var goatImage = Plugin.TextureProvider.GetFromFile(goatImagePath).GetWrapOrDefault();
                if (goatImage != null)
                {
                    using (ImRaii.PushIndent(55f))
                    {
                        ImGui.Image(goatImage.Handle, goatImage.Size);
                    }
                }
                else
                {
                    ImGui.Text("Image not found.");
                }

                ImGuiHelpers.ScaledDummy(20.0f);

                // Example for other services that Dalamud provides.
                // PlayerState provides a wrapper filled with information about the player character.

                var playerState = Plugin.PlayerState;
                if (!playerState.IsLoaded)
                {
                    ImGui.Text("Our local player is currently not logged in.");
                    return;
                }
                
                if (!playerState.ClassJob.IsValid)
                {
                    ImGui.Text("Our current job is currently not valid.");
                    return;
                }

                // If you want to see the Macro representation of this SeString use `.ToMacroString()`
                // More info about SeStrings: https://dalamud.dev/plugin-development/sestring/
                ImGui.Text($"Our current job is ({playerState.ClassJob.RowId}) '{playerState.ClassJob.Value.Abbreviation}' with level {playerState.Level}");

                // Example for querying Lumina, getting the name of our current area.
                var territoryId = Plugin.ClientState.TerritoryType;
                if (Plugin.DataManager.GetExcelSheet<TerritoryType>().TryGetRow(territoryId, out var territoryRow))
                {
                    ImGui.Text($"We are currently in ({territoryId}) '{territoryRow.PlaceName.Value.Name}'");
                }
                else
                {
                    ImGui.Text("Invalid territory.");
                }
            }
        }
    }
}
