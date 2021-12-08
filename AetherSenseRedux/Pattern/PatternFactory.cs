using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Pattern
{
    internal class PatternFactory
    {
        public static IPattern GetPatternFromString(string name, PatternConfig settings)
        {
            switch (name)
            {
                case "Constant":
                    return new ConstantPattern((ConstantPatternConfig)settings);
                case "Ramp":
                    return new RampPattern((RampPatternConfig)settings);
                case "Random":
                    return new RandomPattern((RandomPatternConfig)settings);
                case "Square":
                    return new SquarePattern((SquarePatternConfig)settings);
                default:
                    throw new ArgumentException(String.Format("Invalid pattern {0} specified",name));
            }
        }

        public static PatternConfig GetDefaultsFromString(string name)
        {
            switch (name)
            {
                case "Constant":
                    return ConstantPattern.GetDefaultConfiguration();
                case "Ramp":
                    return RampPattern.GetDefaultConfiguration();
                case "Random":
                    return RandomPattern.GetDefaultConfiguration();
                case "Square":
                    return SquarePattern.GetDefaultConfiguration();
                default:
                    throw new ArgumentException(String.Format("Invalid pattern {0} specified", name));
            }
        }
    }
}
