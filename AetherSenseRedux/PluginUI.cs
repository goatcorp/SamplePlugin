using AetherSenseRedux.Pattern;
using AetherSenseRedux.Trigger;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AetherSenseRedux
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Configuration configuration;
        private Plugin plugin;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return settingsVisible; }
            set { settingsVisible = value; }
        }

        private int SelectedTrigger = 0;

        private Configuration? WorkingCopy;

        // passing in the image here just for simplicity
        public PluginUI(Configuration configuration, Plugin plugin)
        {
            this.configuration = configuration;
            this.plugin = plugin;
        }

        public void Dispose()
        {

        }

        public void Draw()
        {
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawSettingsWindow();
        }

        public void DrawChatTriggerConfig(int i)
        {
            if (WorkingCopy!.Triggers.Count == 0)
            {
                ImGui.Text("Use the Add New button to add a trigger.");
                return;
            }
            dynamic t = WorkingCopy.Triggers[i];
            if (ImGui.BeginTabBar("TriggerConfig", ImGuiTabBarFlags.None))
            {
                if (ImGui.BeginTabItem("Basic"))
                {

                    //begin name field
                    var name = (string)t.Name;
                    if (ImGui.InputText("Name", ref name, 64))
                    {
                        t.Name = name;
                    }
                    //end name field

                    //begin regex field
                    var regex = (string)t.Regex;
                    if (ImGui.InputText("Regex", ref regex, 255))
                    {
                        t.Regex = regex;
                    }
                    //end regex field

                    //begin retrigger delay field
                    var retriggerdelay = (int)t.RetriggerDelay;
                    if (ImGui.InputInt("Retrigger Delay (ms)", ref retriggerdelay))
                    {
                        t.RetriggerDelay = (long)retriggerdelay;
                    }
                    //end retrigger delay field

                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Devices"))
                {
                    //Begin enabled devices selection
                    WorkingCopy.SeenDevices = new List<string>(configuration.SeenDevices);
                    if (WorkingCopy.SeenDevices.Count > 0)
                    {
                        bool[] selected = new bool[WorkingCopy.SeenDevices.Count];
                        bool modified = false;
                        foreach (var (device, j) in WorkingCopy.SeenDevices.Select((value, i) => (value, i)))
                        {
                            if (t.EnabledDevices.Contains(device))
                            {
                                selected[j] = true;
                            }
                            else
                            {
                                selected[j] = false;
                            }
                        }
                        if (ImGui.BeginListBox("Enabled Devices"))
                        {
                            foreach (var (device, j) in WorkingCopy.SeenDevices.Select((value, i) => (value, i)))
                            {
                                if (ImGui.Selectable(device, selected[j]))
                                {
                                    selected[j] = !selected[j];
                                    modified = true;
                                }
                            }
                            ImGui.EndListBox();
                        }
                        if (modified)
                        {
                            var toEnable = new List<string>();
                            foreach (var (device, j) in WorkingCopy.SeenDevices.Select((value, i) => (value, i)))
                            {
                                if (selected[j])
                                {
                                    toEnable.Add(device);
                                }
                            }
                            t.EnabledDevices = toEnable;
                        }
                    } else
                    {
                        ImGui.Text("Connect to Intiface and connect devices to populate the list.");
                    }
                    //end enabled devices selection
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Pattern"))
                {
                    string[] patterns = { "Constant", "Ramp", "Random", "Square"};

                    if (ImGui.BeginCombo("##combo",t.PatternSettings.Type))
                    {
                        foreach (string pattern in patterns)
                        {
                            bool is_selected = t.PatternSettings.Type == pattern;
                            if(ImGui.Selectable(pattern, is_selected))
                            {
                                if (t.PatternSettings.Type != pattern)
                                {
                                    t.Pattern = pattern;
                                    t.PatternSettings = PatternFactory.GetDefaultsFromString(pattern);
                                }
                            }
                            if (is_selected)
                            {
                                ImGui.SetItemDefaultFocus();
                            }
                        }
                        ImGui.EndCombo();
                    }
                    ImGui.SameLine();
                    if (ImGui.ArrowButton("test", ImGuiDir.Right))
                    {
                        plugin.DoPatternTest(t.PatternSettings);
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("Preview pattern on all devices.");
                    }
                    ImGui.Indent();
                    switch ((string)t.PatternSettings.Type)
                    {
                        case "Constant":
                            DrawConstantPatternSettings(t.PatternSettings);
                            break;
                        case "Ramp":
                            DrawRampPatternSettings(t.PatternSettings);
                            break;
                        case "Random":
                            DrawRandomPatternSettings(t.PatternSettings);
                            break;
                        case "Square":
                            DrawSquarePatternSettings(t.PatternSettings);
                            break;
                        default:
                            ImGui.Text("Select a valid pattern.");
                            break;
                    }
                    ImGui.Unindent();

                    ImGui.EndTabItem();
                }
                
                ImGui.EndTabBar();
            }
        }

        public void DrawConstantPatternSettings(dynamic pattern)
        {
            int duration = (int)pattern.Duration;
            if (ImGui.InputInt("Duration (ms)", ref duration))
            {
                pattern.Duration = (long)duration;
            }
            double level = (double)pattern.Level;
            if (ImGui.InputDouble("Level", ref level))
            {
                pattern.Level = level;
            }
        }

        public void DrawRampPatternSettings(dynamic pattern)
        {
            int duration = (int)pattern.Duration;
            if (ImGui.InputInt("Duration (ms)", ref duration))
            {
                pattern.Duration = (long)duration;
            }
            double start = (double)pattern.Start;
            if (ImGui.InputDouble("Start", ref start))
            {
                pattern.Start = start;
            }
            double end = (double)pattern.End;
            if (ImGui.InputDouble("End", ref end))
            {
                pattern.End = end;
            }
        }

        public void DrawRandomPatternSettings(dynamic pattern)
        {
            int duration = (int)pattern.Duration;
            if (ImGui.InputInt("Duration (ms)", ref duration))
            {
                pattern.Duration = (long)duration;
            }
            double min = (double)pattern.Minimum;
            if (ImGui.InputDouble("Minimum", ref min))
            {
                pattern.Minimum = min;
            }
            double max = (double)pattern.Maximum;
            if (ImGui.InputDouble("Maximum", ref max))
            {
                pattern.Maximum = max;
            }
        }

        public void DrawSquarePatternSettings(dynamic pattern)
        {
            int duration = (int)pattern.Duration;
            if (ImGui.InputInt("Duration (ms)", ref duration))
            {
                pattern.Duration = (long)duration;
            }
            double level1 = (double)pattern.Level1;
            if (ImGui.InputDouble("Level 1", ref level1))
            {
                pattern.Level1 = level1;
            }
            int duration1 = (int)pattern.Duration1;
            if (ImGui.InputInt("Level 1 Duration (ms)", ref duration1))
            {
                pattern.Duration1 = (long)duration1;
            }
            double level2 = (double)pattern.Level2;
            if (ImGui.InputDouble("Level 2", ref level2))
            {
                pattern.Level2 = level2;
            }
            int duration2 = (int)pattern.Duration2;
            if (ImGui.InputInt("Level 2 Duration (ms)", ref duration2))
            {
                pattern.Duration2 = (long)duration2;
            }
            int offset = (int)pattern.Offset;
            if (ImGui.InputInt("Offset (ms)", ref offset))
            {
                pattern.Offset = (long)offset;
            }
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {

                if (WorkingCopy != null)
                {
                    PluginLog.Debug("Making WorkingCopy null at settingsvisible statement,");
                    WorkingCopy = null;
                }
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(640, 400), ImGuiCond.Appearing);
            if (ImGui.Begin("AetherSense Redux", ref settingsVisible, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar))
            {
                if (WorkingCopy == null)
                {
                    PluginLog.Debug("WorkingCopy was null, importing current config.");
                    WorkingCopy = new Configuration();
                    WorkingCopy.Import(configuration);
                }
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("Buttplug"))
                    {
                        if (plugin.Running)
                        {
                            if (ImGui.MenuItem("Disconnect")){
                                plugin.Stop();
                            }
                        } else
                        {
                            if (ImGui.MenuItem("Connect")){
                                plugin.Start();
                            }
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenuBar();
                }
                if (ImGui.BeginChild("body", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()), false))
                {
                    ImGui.Indent(1);
                    if (ImGui.BeginTabBar("MyTabBar", ImGuiTabBarFlags.None))
                    {
                        if (ImGui.BeginTabItem("Basic"))
                        {
                            var address = WorkingCopy.Address;
                            if (ImGui.InputText("Intiface Address", ref address, 64))
                            {
                                WorkingCopy.Address = address;
                            }


                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Triggers"))
                        {
                            var listToRemove = new List<dynamic>();
                            if (SelectedTrigger >= WorkingCopy.Triggers.Count)
                            {
                                SelectedTrigger = (SelectedTrigger > 0) ? WorkingCopy.Triggers.Count - 1 : 0;
                            }
                            if (ImGui.BeginChild("left", new Vector2(150, -ImGui.GetFrameHeightWithSpacing()), true))
                            {
                                foreach (var (t, i) in WorkingCopy.Triggers.Select((value, i) => (value, i)))
                                {
                                    if (ImGui.Selectable(String.Format("{0} ({1})", t.Name, t.Type), SelectedTrigger == i))
                                    {
                                        SelectedTrigger = i;
                                    }

                                }
                                ImGui.EndChild();
                            }
                            ImGui.SameLine();
                            if (ImGui.BeginChild("right", new Vector2(0, -(ImGui.GetFrameHeightWithSpacing()*2)), false))
                            {
                                DrawChatTriggerConfig(SelectedTrigger);
                                ImGui.EndChild();
                            }



                            if (ImGui.Button("Add New"))
                            {
                                List<dynamic> triggers = WorkingCopy.Triggers;
                                triggers.Add(new ChatTriggerConfig()
                                {
                                    Pattern = "Constant",
                                    PatternSettings = new ConstantPatternConfig()
                                });
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("Remove"))
                            {
                                WorkingCopy.Triggers.RemoveAt(SelectedTrigger);
                            }
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Advanced"))
                        {
                            var configValue = WorkingCopy.LogChat;
                            if (ImGui.Checkbox("Log Chat to Debug", ref configValue))
                            {
                                WorkingCopy.LogChat = configValue;

                            }
                            if (ImGui.Button("Restore Default Triggers"))
                            {
                                WorkingCopy.LoadDefaults();
                            }
                            ImGui.EndTabItem();
                        }
                        ImGui.EndTabBar();
                    }
                    ImGui.Unindent(1);
                    ImGui.EndChild();
                }
                if (ImGui.Button("Save"))
                {
                    configuration.Import(WorkingCopy);
                    configuration.Save();
                    plugin.Restart();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Save configuration changes to disk.");
                }
                ImGui.SameLine();
                if (ImGui.Button("Apply"))
                {
                    configuration.Import(WorkingCopy);
                    plugin.Restart();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Apply configuration changes without saving.");
                }
                ImGui.SameLine();
                if (ImGui.Button("Revert"))
                {
                    try
                    {
                        var cloneconfig = configuration.CloneConfigurationFromDisk();
                        configuration.Import(cloneconfig);
                        WorkingCopy.Import(configuration);
                    } catch (Exception ex)
                    {
                        PluginLog.Error(ex, "Could not restore configuration.");
                    }

                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Discard all changes and reload the configuration from disk.");
                }
            }

            ImGui.End();
        }
    }
}
