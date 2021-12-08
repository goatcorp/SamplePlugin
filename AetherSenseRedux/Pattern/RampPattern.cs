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
        private readonly double startLevel;
        private readonly double endLevel;
        private readonly long duration;


        public RampPattern(RampPatternConfig config)
        {
            startLevel = config.Start;
            endLevel = config.End;
            this.duration = config.Duration;
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);
        }

        public double GetIntensityAtTime(DateTime time)
        {
            if (Expires < time)
            {
                throw new PatternExpiredException();
            }
            double progress = (Expires.Ticks - time.Ticks) / TimeSpan.FromMilliseconds(duration).Ticks;
            return (endLevel - startLevel) * progress + startLevel;
        }

        public static PatternConfig GetDefaultConfiguration()
        {
            return new RampPatternConfig();
        }
    }
    [Serializable]
    public class RampPatternConfig : PatternConfig
    {
        public override string Type { get; } = "Ramp";
        public double Start { get; set; } = 0;
        public double End { get; set; } = 1;
    }
}
