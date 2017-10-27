namespace AcmeAirAnalysis.Model
{
    class Request
    {
        public int Id { get; set; }
        public int ImportId { get; set; }
        public string Protocol { get; set; }
        public int Latency { get; set; }
        public string Timestamp { get; set; }
        public int ResponseTime { get; set; }
        public string TestName { get; set; }
        public string ResponseCode { get; set; }
        public int ResponseLength { get; set; }
    }
}
