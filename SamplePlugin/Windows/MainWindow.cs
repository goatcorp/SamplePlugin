using Dalamud.Interface.Windowing;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
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
    public void GeneratePlayers(string playerName)
    {
        ImGui.Text(playerName);
        ImGui.SameLine();
        ImGui.InputInt("some value", ref player1Bet, 1, 2, 0);
        ImGui.SameLine();
        if (ImGui.Button("This is a button"))
        {
            // Do something when pressed -> Hit a card
        }
    }
    public override void Draw()
    {
        string host = "Moonhell";
        int totalBet = player1Bet + player2Bet;
        ImGui.Text($"This is your main window. You won't need any more window if you plan on using tabs.");
        if (ImGui.Button("This is a button"))
        {
            // Do something when pressed: enable disable plugin
        }
        ImGui.BeginTable("Table1", 2);
    ImGui.TableSetupColumn("Player");
    ImGui.TableSetupColumn("Bets");
    ImGui.TableSetupColumn("Cards");
    ImGui.TableNextRow();
        ImGui.Text(host);
        ImGui.TableNextColumn();
        ImGui.Text("total bet");
        ImGui.TableNextColumn();
        if (ImGui.Button("This is a button"))
        {
            // Do something when pressed
        }
        ImGui.TableNextRow();
        for (int i = 1; i <= 8; i++)
        {
            GeneratePlayers("Player " + i);
            ImGui.TableNextRow();
        }

        ImGui.EndTable();
    }
}
