using System;
using System.Collections.Generic;

namespace AetherSenseRedux
{
    internal class ConstantPattern : IPattern
    {
        public DateTime Expires { get; set; }
        private double level;

        public ConstantPattern(long duration, Dictionary<string,object> config)
        {
            this.level = (double)config["level"];
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);
        } 

        public double GetIntensityAtTime(DateTime time)
        {
            if (this.Expires < time)
            {
                throw new Exception();
            }
            return this.level;
        }
    }

    internal class RampPattern : IPattern
    {
        public DateTime Expires { get; set; }
        private double minLevel;
        private double maxLevel;
        private long duration;


        public RampPattern(long duration, Dictionary<string, object> config)
        {
            minLevel = (double)config["min"];
            maxLevel = (double)config["max"];
            this.duration = duration;
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);
        }

        public double GetIntensityAtTime(DateTime time)
        {
            if (this.Expires < time)
            {
                throw new Exception();
            }
            double progress = (Expires.Ticks - time.Ticks)/ TimeSpan.FromMilliseconds(duration).Ticks;
            return (maxLevel - minLevel) * progress + minLevel;
        }
    }
}
