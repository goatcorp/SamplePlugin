using Dalamud.Interface.Windowing;
using ImGuiNET;
using Newtonsoft.Json.Linq;
using System;
using System.Numerics;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private int player1Bet = 0;
    private int player2Bet = 0;
    public MainWindow() : base("Baccarat by Moonhell", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
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
        string host = "Moonhell";
        string player1 = "Kestra";
        string player2 = "Klyhia";
        
        int totalBet = player1Bet+ player2Bet;
        if (ImGui.Button("This is a button"))
        {
            // Do something when pressed: enable disable plugin
        }
        ImGui.Text(host);
        ImGui.SameLine();
        ImGui.Text("total bet");
        ImGui.SameLine();
        if (ImGui.Button("This is a button"))
        {
            // Do something when pressed
        }
        ImGui.NewLine();
        ImGui.Text(player1);
        ImGui.SameLine();
        ImGui.InputInt("some value", ref player1Bet, 1, 2, 0);
        ImGui.SameLine();
        if (ImGui.Button("This is a button"))
        {
            // Do something when pressed -> Hit a card
        }
        ImGui.NewLine();
        ImGui.Text(player2);
        ImGui.SameLine();
        ImGui.InputInt("some value", ref player2Bet, 1, 2, 0);
        ImGui.SameLine();
        if (ImGui.Button("This is a button"))
        {
            // Do something when pressed -> Hit a card
        }

    }
}
