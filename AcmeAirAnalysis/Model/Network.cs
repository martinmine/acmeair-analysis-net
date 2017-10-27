namespace AcmeAirAnalysis.Model
{
    class Network
    {
        public int Id { get; set; }
        public int ImportId { get; set; }
        public string Protocol { get; set; }
        public int Latency { get; set; }
        public string Servicename { get; set; }
        public string Interface { get; set; }
        public int TxBytes { get; set; }
        public int TxPackets { get; set; }
        public int RxBytes { get; set; }
        public int RxPackets { get; set; }
    }
}
