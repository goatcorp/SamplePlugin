using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Pattern
{
    internal class RandomPattern : IPattern
    {
        public DateTime Expires { get; set; }
        private readonly Random rand = new Random();
        private readonly double min;
        private readonly double max;

        public RandomPattern(RandomPatternConfig config)
        {
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds(config.Duration);
            min = config.Minimum;
            max = config.Maximum;
        }

        public double GetIntensityAtTime(DateTime time)
        {
            if (Expires < time)
            {
                throw new PatternExpiredException();
            }
            return Scale(rand.NextDouble(),min,max);
        }
        private static double Scale(double value, double min, double max)
        {
            return value * (max - min) + min;
        }

        public static PatternConfig GetDefaultConfiguration()
        {
            return new RandomPatternConfig();
        }
    }
    [Serializable]
    public class RandomPatternConfig : PatternConfig
    {
        public double Minimum { get; set; } = 0;
        public double Maximum { get; set; } = 1;
    }
}
