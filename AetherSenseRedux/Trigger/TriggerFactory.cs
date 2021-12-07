using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Trigger
{
    internal class TriggerFactory
    {
        public static ITrigger GetTriggerFromConfig(Dictionary<string,dynamic> config, ref List<Device> devices)
        {
            switch (config["type"])
            {
                case "Chat":
                    return new ChatTrigger(
                            config["name"],
                            ref devices,
                            config["enabledDevices"],
                            config["pattern"],
                            config["patternSettings"],
                            config["regex"],
                            config["retriggerDelay"]
                        );
                default:
                    throw new ArgumentException(String.Format("Invalid trigger {0} specified", config["name"]));
            }
        }
    }
}
