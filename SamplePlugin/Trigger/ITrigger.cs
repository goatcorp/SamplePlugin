using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Trigger
{
    internal interface ITrigger
    {
        bool Enabled { get; set; }
        string Name { get; init; }
        List<Device> Devices { get; init; }
        List<string> EnabledDevices { get; init; }
        string Pattern { get; init; }
        Dictionary<string, object> PatternSettings { get; init; }

        void OnTrigger();
        Task Run();
    }
}
