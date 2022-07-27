using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace SamplePlugin.Windows; 

public class MainWindow : Window, IDisposable {
    private TextureWrap goatImage;
    private Configuration configuration;
    private WindowSystem windowSystem;

    public MainWindow(TextureWrap goatImage, Configuration configuration, WindowSystem windowSystem) : base("My Amazing Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse) {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.goatImage = goatImage;
        this.configuration = configuration;
        this.windowSystem = windowSystem;
    }

    public void Dispose() {
        this.goatImage.Dispose();
    }

    public override void Draw() {
        ImGui.Text($"The random config bool is {this.configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings")) {
            this.windowSystem.GetWindow("A Wonderful Configuration Window").IsOpen = true;
        }

        ImGui.Spacing();

        ImGui.Text("Have a goat:");
        ImGui.Indent(55);
        ImGui.Image(this.goatImage.ImGuiHandle, new Vector2(this.goatImage.Width, this.goatImage.Height));
        ImGui.Unindent(55);
    }
}
