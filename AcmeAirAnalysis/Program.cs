using AcmeAirAnalysis.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcmeAirAnalysis
{
    class Program
    {
        static string[] testNames = { "Login", "QueryFlight", "List Bookings", "logout", "BookFlight",
                "View Profile Information", "Update Customer", "Cancel Booking" };

        private static DataSetDao dao = new DataSetDao("server=localhost;user=root;database=acmeair;port=3306;password=123;SslMode=none");
        private static string nowString = DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss");
        private static int[] latencies = { 0, 2, 5 };
        private static string[] protocols = { "coap", "h2c", "http1.1" };


        static void Main(string[] args)
        {
            string setDate = "2017-10-27";
            //WriteRequestAnalysis(setDate);
            //WriteNetworkAnalysis(setDate);
            WriteUsageAnalysis(setDate);
        }

        private static void WriteRequestAnalysis(string setDate)
        {
            DataSet set = dao.GetByDate(setDate);

            var fileWriter = new InvertedCSVFile();

            List<string> header = new List<string>();
            header.Add("Protocol");
            header.Add("Latency");
            header.AddRange(testNames.ToList());

            fileWriter.AddList(header);

            foreach (var latency in latencies)
            {
                foreach (var protocol in protocols)
                {
                    DoRequestAnalysis(latency, protocol, set.Id, dao, fileWriter);
                }
            }

            string filename = $"results-{nowString}.csv";
            fileWriter.WriteToFile(filename);

            Console.WriteLine("Written results to " + filename);
        }

        private static void WriteNetworkAnalysis(string setDate)
        {
            string[] headers = { "Protocol", "Latency", "Service", "Tx bytes", "Tx packets", "Rx bytes", "Rx Packets" };
            DataSet set = dao.GetByDate(setDate);

            var fileWriter = new InvertedCSVFile();
            fileWriter.AddColumn(headers);

            foreach (var latency in latencies)
            {
                foreach (var protocol in protocols)
                {
                    foreach (var record in dao.GetNetworkRecords(set.Id, protocol, latency))
                    {
                        fileWriter.AddColumn(protocol, latency, record.Servicename, record.TxBytes, record.TxPackets, record.RxBytes, record.RxPackets);
                    }
                }
            }

            string filename = $"network-{nowString}.csv";
            fileWriter.WriteToFile(filename);

            Console.WriteLine("Written results to " + filename);
        }

        private static void WriteUsageAnalysis(string setDate)
        {
            string[] headers = { "Protocol", "Latency", "Service", "Usage" };
            DataSet set = dao.GetByDate(setDate);

            var fileWriter = new InvertedCSVFile();
            fileWriter.AddColumn(headers);

            foreach (var latency in latencies)
            {
                foreach (var protocol in protocols)
                {
                    foreach (var record in dao.GetCpuUsage(set.Id, protocol, latency))
                    {
                        fileWriter.AddColumn(protocol, latency, record.Service, record.Usage);
                    }
                }
            }

            string filename = $"usage-{nowString}.csv";
            fileWriter.WriteToFile(filename);

            Console.WriteLine("Written results to " + filename);
        }

        private static void DoRequestAnalysis(int latency, string protocol, int setId, DataSetDao dao, InvertedCSVFile file)
        {
            Console.Write($"Analyzing requests for protocol {protocol}/{latency}ms -> ");

            var stats = dao.GetStats(setId, protocol, latency);

            Append(protocol, latency, stats, file, t => t.Mean);
            Append(protocol, latency, stats, file, t => t.StandardDeviation);
            //Append(protocol, latency, stats, file, t => GetMedian(latency, protocol, setId, dao));

            Console.WriteLine("DONE!");
        }

        private static double GetMedian(int latency, string protocol, int setId, DataSetDao dao)
        {
            var items = new List<int>();
            var requests = dao.GetRequestRecords(setId, protocol, latency);

            foreach (var record in requests)
            {
                items.Add(record.ResponseTime);
            }

            items.Sort();
            if (items.Count % 2 == 0)
                return (items.ElementAt(items.Count / 2 - 1) + items.ElementAt(items.Count / 2)) / 2.0;
            return items.ElementAt(items.Count / 2);
        }

        private static void Append(string protocol, int latency, Dictionary<string, StatEntry> stats, InvertedCSVFile file, Func<StatEntry, object> selector)
        {
            var fileEntryErr = new List<string>();

            fileEntryErr.Add(protocol);
            fileEntryErr.Add(latency.ToString());

            foreach (var testName in testNames)
            {
                fileEntryErr.Add(selector(stats[testName]).ToString());
            }

            file.AddList(fileEntryErr);
        }
    }
}
