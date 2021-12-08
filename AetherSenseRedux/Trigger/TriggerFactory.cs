using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AetherSenseRedux.Trigger
{
    internal class TriggerFactory
    {
        public static ITrigger GetTriggerFromConfig(TriggerConfig config, ref List<Device> devices)
        {
            switch (config.Type)
            {
                case "Chat":
                    return new ChatTrigger((ChatTriggerConfig)config, ref devices);
                default:
                    throw new ArgumentException(String.Format("Invalid trigger {0} specified", config.Type));
            }
        }

        public static TriggerConfig GetTriggerConfigFromObject(dynamic o)
        {
            switch ((string)o.Type)
            {
                case "Chat":
                    return new ChatTriggerConfig()
                    {
                        Name = (string)o.Name,
                        Regex = (string)o.Regex,
                        RetriggerDelay = (long)o.RetriggerDelay,
                        EnabledDevices = (List<string>)o.EnabledDevices,
                        Pattern = (string)o.Pattern,
                        PatternSettings = (dynamic)o.PatternSettings
                    };
                default:
                    throw new ArgumentException(String.Format("Invalid trigger {0} specified", o.Type));
            }
        }
    }
}
