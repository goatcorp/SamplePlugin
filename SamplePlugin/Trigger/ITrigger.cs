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
        List<Device> _devices { get; init; }
        List<string> _enabledDevices { get; init; }
        string _pattern { get; init; }
        Dictionary<string, object> _patternSettings { get; init; }

        void OnTrigger();
        Task Run();
    }
}
