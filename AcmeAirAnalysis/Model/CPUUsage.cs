using System;

namespace AcmeAirAnalysis.Model
{
    class CpuUsage
    {
        public int ImportId { get; set; }
        public DateTime Time { get; set; }
        public string Service { get; set; }
        public int Latency { get; set; }
        public string Protocol { get; set; }
        public float Usage { get; set; }
    }
}
