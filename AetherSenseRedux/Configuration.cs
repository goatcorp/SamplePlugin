using AetherSenseRedux.Pattern;
using AetherSenseRedux.Trigger;
using Dalamud.Configuration;
using Dalamud.Logging;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace AetherSenseRedux
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;
        public bool Initialized = false;

        public bool LogChat { get; set; } = false;

        public bool Enabled { get; set; } = false;
        public string Address { get; set; } = "ws://127.0.0.1:12345";

        public List<string> SeenDevices { get; set; } = new();

        public List<dynamic> Triggers { get; set; } = new List<dynamic>();

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void FixDeserialization()
        {
            List<TriggerConfig> triggers = new List<TriggerConfig>();
            foreach (dynamic t in Triggers)
            {
                triggers.Add(TriggerFactory.GetTriggerConfigFromObject(t));
            }
            Triggers = new List<dynamic>();

            foreach (TriggerConfig t in triggers)
            {
                Triggers.Add(t);
            }
        }

        public void LoadDefaults()
        {
            Version = 1;
            Initialized = true;
            Triggers = new List<dynamic>() {
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

        public void Import(dynamic o)
        {
            try
            {
                if (o.Version != 1)
                {
                    return;
                }
                Version = o.Version;
                Initialized = o.Initialized;
                LogChat = o.LogChat;
                Address = o.Address;
                SeenDevices = new List<string>(o.SeenDevices);
                Triggers = o.Triggers;
                FixDeserialization();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Attempted to import a bad configuration.");
            }
        }
        public Configuration CloneConfigurationFromDisk()
        {
            if (pluginInterface == null)
            {
                throw new NullReferenceException("Attempted to load the plugin configuration from a clone.");
            }
            var config = pluginInterface!.GetPluginConfig() as Configuration ?? throw new NullReferenceException("No configuration exists on disk.");
            config.FixDeserialization();
            return config;
        }
    }
}
