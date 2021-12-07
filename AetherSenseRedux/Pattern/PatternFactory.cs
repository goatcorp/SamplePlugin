using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Pattern
{
    internal class PatternFactory
    {
        public static IPattern GetPatternFromString(string name, Dictionary<string, dynamic> settings)
        {
            switch (name)
            {
                case "Constant":
                    return new ConstantPattern(settings);
                case "Ramp":
                    return new RampPattern(settings);
                case "Random":
                    return new RandomPattern(settings);
                case "Square":
                    return new SquarePattern(settings);
                default:
                    throw new ArgumentException(String.Format("Invalid pattern {0} specified",name));
            }
        }

        public static Dictionary<string,dynamic> GetDefaultsFromString(string name)
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
