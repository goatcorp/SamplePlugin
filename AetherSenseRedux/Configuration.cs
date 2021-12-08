using AetherSenseRedux.Pattern;
using AetherSenseRedux.Trigger;
using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace AetherSenseRedux
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool LogChat { get; set; } = false;

        public bool Enabled { get; set; } = false;
        public string Address { get; set; } = "ws://127.0.0.1:12345";

        public List<string> SeenDevices { get; set; } = new();

        public List<ChatTriggerConfig> Triggers { get; set; } = new List<ChatTriggerConfig>();

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void LoadDefaults()
        {
            Triggers = new List<ChatTriggerConfig>() {
                new ChatTriggerConfig()
                {
                    Name = "Casted",
                    EnabledDevices = new List<string>(),
                    Pattern = "Constant",
                    PatternSettings = new ConstantPatternConfig()
                    {
                        Level = 1,
                        Duration = 200
                    },
                    Regex = "You cast",
                    RetriggerDelay = 0
                },
                new ChatTriggerConfig()
                {

                    Name = "Casting",
                    EnabledDevices = new List<string>(),
                    Pattern = "Ramp",
                    PatternSettings = new RampPatternConfig()
                    {
                        Start = 0,
                        End = 0.75,
                        Duration = 2500
                    },
                    Regex = "You begin casting",
                    RetriggerDelay = 250
                }
            };
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}
