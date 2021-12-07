
using System;
using System.Collections.Generic;

namespace AetherSenseRedux.Pattern
{
    internal class ConstantPattern : IPattern
    {
        public DateTime Expires { get; set; }
        private readonly double level;

        public ConstantPattern(Dictionary<string, dynamic> config)
        {
            level = (double)config["level"];
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds((long)config["duration"]);
        }

        public double GetIntensityAtTime(DateTime time)
        {
            if (Expires < time)
            {
                throw new PatternExpiredException();
            }
            return level;
        }
        public static Dictionary<string, dynamic> GetDefaultConfiguration()
        {
            return new Dictionary<string, dynamic>
            {
                {"level", 1 },
                {"duration", 1000 }
            };
        }
    }
}
