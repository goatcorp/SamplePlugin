using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private string GoatImagePath;
    private Plugin Plugin;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, string goatImagePath)
        : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        GoatImagePath = goatImagePath;
        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings"))
        {
            Plugin.ToggleConfigUI();
        }

        ImGui.Spacing();

        ImGui.Text("Have a goat:");
        var goatImage = Plugin.TextureProvider.GetFromFile(GoatImagePath).GetWrapOrDefault();
        if (goatImage != null)
        {
            ImGuiHelpers.ScaledIndent(55f);
            ImGui.Image(goatImage.ImGuiHandle, new Vector2(goatImage.Width, goatImage.Height));
            ImGuiHelpers.ScaledIndent(-55f);
        }
        else
        {
            ImGui.Text("Image not found.");
        }
    }
}
