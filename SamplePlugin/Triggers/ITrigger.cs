using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Triggers
{
    internal interface ITrigger
    {
        bool Enabled { get; set; }
    }
}
