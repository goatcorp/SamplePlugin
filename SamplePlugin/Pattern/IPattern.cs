using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Pattern
{

    internal interface IPattern
    {
        DateTime Expires { get; set; }
        double GetIntensityAtTime(DateTime currTime);

    }
}