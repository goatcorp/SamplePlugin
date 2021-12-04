using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Pattern
{
    internal class PatternFactory
    {
        public static IPattern GetPatternFromString(string name, Dictionary<string, object> settings)
        {
            switch (name)
            {
                case "Constant":
                    return new ConstantPattern(settings);
                case "Ramp":
                    return new RampPattern(settings);
                default:
                    throw new ArgumentException();
            }
        }
    }
}
