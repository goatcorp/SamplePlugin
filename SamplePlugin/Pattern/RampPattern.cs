using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Pattern
{
    internal class RampPattern : IPattern
    {
        public DateTime Expires { get; set; }
        private readonly double minLevel;
        private readonly double maxLevel;
        private readonly long duration;


        public RampPattern(Dictionary<string, object> config)
        {
            minLevel = (double)config["min"];
            maxLevel = (double)config["max"];
            this.duration = (long)config["duration"];
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);
        }

        public double GetIntensityAtTime(DateTime time)
        {
            if (Expires < time)
            {
                throw new PatternExpiredException();
            }
            double progress = (Expires.Ticks - time.Ticks) / TimeSpan.FromMilliseconds(duration).Ticks;
            return (maxLevel - minLevel) * progress + minLevel;
        }
    }
}
