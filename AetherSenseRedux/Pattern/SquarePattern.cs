using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Pattern
{
    internal class SquarePattern : IPattern
    {
        public DateTime Expires { get; set; }
        private readonly double level1;
        private readonly double level2;
        private readonly long duration1;
        private readonly long offset;
        private readonly long total_duration;


        public SquarePattern(Dictionary<string, dynamic> config)
        {
            level1 = (double)config["level1"];
            level2 = (double)config["level2"];
            duration1 = (long)config["duration1"];
            offset = (long)config["offset"];
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds((long)config["duration"]);
            total_duration = duration1 + (long)config["duration2"];
        }

        public double GetIntensityAtTime(DateTime time)
        {
            if (Expires < time)
            {
                throw new PatternExpiredException();
            }
            long patternTime = DateTime.UtcNow.Ticks / 10000 + offset;

            long progress = patternTime % total_duration;

            return (progress < duration1)? level1 : level2;
        }
        public static Dictionary<string, dynamic> GetDefaultConfiguration()
        {
            return new Dictionary<string, dynamic>
            {
                {"level1", 0 },
                {"level2", 1 },
                {"duration1", 250 },
                {"duration2", 250 },
                {"offset", 0 },
                {"duration", 1000 }
            };
        }
    }
}
