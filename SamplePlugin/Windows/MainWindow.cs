using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    public MainWindow() : base("My Amazing Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        ImGui.Text($"This is your main window. You won't need any more window if you plan on using tabs.");

        if (ImGui.Button("This is a button"))
        {
            // Do something when pressed
        }

        // There's alot more ways to interact, it's all under ImGui.<something>. Play with it !
    }
}
