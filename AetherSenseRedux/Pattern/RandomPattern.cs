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

        public RandomPattern(Dictionary<string, dynamic> config)
        {
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds((long)config["duration"]);
            min = (double)config["min"];
            max = (double)config["max"];
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

        public static Dictionary<string, dynamic> GetDefaultConfiguration()
        {
            return new Dictionary<string, dynamic>
            {
                {"min", 0 },
                {"max", 1 },
                {"duration", 1000 }
            };
        }
    }
}
