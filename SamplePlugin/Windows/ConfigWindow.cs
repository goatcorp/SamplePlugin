using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace SamplePlugin.Windows;

public class ConfigWindow : Window, IDisposable {
    private Configuration configuration;

    public ConfigWindow(Configuration configuration) : base(
        "A Wonderful Configuration Window",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse) {
        this.Size = new Vector2(232, 75);
        this.SizeCondition = ImGuiCond.Always;

        this.configuration = configuration;
    }

    public void Dispose() { }

    public override void Draw() {
        // can't ref a property, so use a local copy
        var configValue = this.configuration.SomePropertyToBeSavedAndWithADefault;
        if (ImGui.Checkbox("Random Config Bool", ref configValue)) {
            this.configuration.SomePropertyToBeSavedAndWithADefault = configValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.configuration.Save();
        }
    }
}
