using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Trigger
{
    internal interface ITrigger
    {
        bool Enabled { get; set; }
        void OnMatch();
        Task Run();
    }
}
