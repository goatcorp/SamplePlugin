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

        public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

        public bool Enabled { get; set; } = false;
        public string Address { get; set; } = "ws://127.0.0.1:12345";

        public List<string> SeenDevices { get; set; } = new();

        public List<Dictionary<string, dynamic>> Triggers { get; set; } = new List<Dictionary<string, dynamic>> 
        { 
            new Dictionary<string, dynamic> 
            {
                { "name", "Cast" },
                { "enabledDevices", new List<string>()},
                { "pattern", "Constant" },
                { "patternSettings", new Dictionary<string, object> 
                    {
                        {"level", 1 },
                        {"duration", 250 }
                    }
                },
                { "regex", "You cast" },
                { "retriggerDelay", 0 }
            },
            new Dictionary<string, dynamic>
            {
                { "name", "Casting" },
                { "enabledDevices", new List<string>()},
                { "pattern", "Ramp" },
                { "patternSettings", new Dictionary<string, object>
                    {
                        {"start", 0 },
                        {"end", 0.75 },
                        {"duration", 2500 }
                    }
                },
                { "regex", "You begin casting" },
                { "retriggerDelay", 250 }
            }
        };

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}
