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


        public SquarePattern(SquarePatternConfig config)
        {
            level1 = config.Level1;
            level2 = config.Level2;
            duration1 = config.Duration1;
            offset = config.Offset;
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds(config.Duration);
            total_duration = duration1 + config.Duration2;
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
        public static PatternConfig GetDefaultConfiguration()
        {
            return new SquarePatternConfig();
        }
    }
    [Serializable]
    public class SquarePatternConfig : PatternConfig
    {
        public override string Type { get; } = "Square";
        public double Level1 { get; set; } = 0;
        public double Level2 { get; set; } = 1;
        public long Duration1 { get; set; } = 200;
        public long Duration2 { get; set; } = 200;
        public long Offset { get; set; } = 0;
    }
}
