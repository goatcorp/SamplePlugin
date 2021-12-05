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

        public RandomPattern(Dictionary<string, object> config)
        {
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds((long)config["duration"]);
        }

        public double GetIntensityAtTime(DateTime time)
        {
            if (Expires < time)
            {
                throw new PatternExpiredException();
            }
            return rand.NextDouble();
        }
    }
}
