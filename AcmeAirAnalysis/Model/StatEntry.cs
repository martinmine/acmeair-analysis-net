using System;
using System.Collections.Generic;
using System.Text;

namespace AcmeAirAnalysis.Model
{
    class StatEntry
    {
        internal int Min { get; set; }
        internal int Max { get; set;  }
        internal double StandardDeviation { get; set; }
        internal decimal Mean { get; set; }
    }
}
