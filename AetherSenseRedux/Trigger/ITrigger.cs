using AetherSenseRedux.Pattern;
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
        string Type { get; }

        Task MainLoop();

        static Dictionary<string, dynamic> GetDefaultConfiguration()
        {
            return new Dictionary<string, dynamic>();
        }
    }
    [Serializable]
    public abstract class TriggerConfig
    {
        public abstract string Type { get; }
        public abstract string Name { get; set; }
        public List<string> EnabledDevices { get; set; } = new List<string>();
        public string Pattern { get; set; } = "";
        public dynamic PatternSettings { get; set; } = null!;
    }
}
