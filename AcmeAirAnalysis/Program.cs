using AcmeAirAnalysis.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcmeAirAnalysis
{
    class Program
    {
        private static string[] testNames = { "Login", "QueryFlight", "List Bookings", "logout", "BookFlight",
                "View Profile Information", "Update Customer", "Cancel Booking" };

        private static string[] cpuServices = { "nginx1", "auth-service-liberty1", "booking-service-liberty1", "customer-service-liberty1", "flight-service-liberty1" };

        private static DataSetDao dao = new DataSetDao("server=localhost;user=root;database=acmeair;port=3306;password=123;SslMode=none");
        private static string nowString = DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss");
        private static int[] latencies = { 0, 2, 5 };
        private static string[] protocols = { "coap", "h2c", "http1.1" };


        static void Main(string[] args)
        {
            string setDate = "2017-10-27";
            WriteRequestAnalysis(setDate);
            string[] services = { "auth-service-liberty1", "booking-service-liberty1", "flight-service-liberty1", "customer-service-liberty1" };
            WriteNetworkAnalysis(setDate, "txBytes", t => t.TxBytes, services);
            WriteNetworkAnalysis(setDate, "txPackets", t => t.TxPackets, services);
            WriteNetworkAnalysis(setDate, "rxBytes", t => t.RxBytes, services);
            WriteNetworkAnalysis(setDate, "rxPackets", t => t.RxPackets, services);

            WriteNetworkAnalysis(setDate, "txBytes-gateway", t => t.TxBytes, "nginx1");
            WriteNetworkAnalysis(setDate, "txPackets-gateway", t => t.TxPackets, "nginx1");
            WriteNetworkAnalysis(setDate, "rxBytes-gateway", t => t.RxBytes, "nginx1");
            WriteNetworkAnalysis(setDate, "rxPackets-gateway", t => t.RxPackets, "nginx1");
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
                    DoRequestAnalysis(latency, protocol, set.Id, dao, fileWriter, (i, p, l) => dao.GetStats(i, p, l), testNames);
                }
            }

            string filename = $"results-{nowString}.csv";
            fileWriter.WriteToFile(filename);

            Console.WriteLine("Written results to " + filename);
        }

        private static void WriteNetworkAnalysis(string setDate, string fileNamePrefix, Func<Network, object> selector, params string[] services)
        {
            string[] headers = { "Protocol" };
            DataSet set = dao.GetByDate(setDate);

            var fileWriter = new InvertedCSVFile();
            fileWriter.AddList(headers.Union(services).ToList());

            foreach (var latency in latencies)
            {
                foreach (var protocol in protocols)
                {
                    List<string> fileRecord = new List<string>();
                    fileRecord.Add($"{protocol} ({latency} ms)");

                    foreach (var serviceName in services)
                    {
                        var record = dao.GetNetworkRecord(set.Id, protocol, latency, serviceName);
                        fileRecord.Add(selector(record).ToString());
                    }

                    fileWriter.AddList(fileRecord);
                }
            }

            string filename = $"{fileNamePrefix}-{nowString}.csv";
            fileWriter.WriteToFile(filename);

            Console.WriteLine("Written results to " + filename);
        }

        private static void WriteUsageAnalysis(string setDate)
        {
            DataSet set = dao.GetByDate(setDate);

            var fileWriter = new InvertedCSVFile();

            List<string> header = new List<string>();
            header.Add("Protocol");
            header.Add("Latency");
            header.AddRange(cpuServices.ToList());

            fileWriter.AddList(header);

            foreach (var latency in latencies)
            {
                foreach (var protocol in protocols)
                {
                    DoRequestAnalysis(latency, protocol, set.Id, dao, fileWriter, (i, p, l) => dao.GetCpuStats(i, p, l), cpuServices);
                }
            }

            string filename = $"usage-{nowString}.csv";
            fileWriter.WriteToFile(filename);

            Console.WriteLine("Written results to " + filename);
        }

        private static void DoRequestAnalysis(int latency, string protocol, int setId, DataSetDao dao, InvertedCSVFile file, Func<int, string, int, Dictionary<string, StatEntry>> selector, string[] entryHeaders)
        {
            Console.Write($"Analyzing requests for protocol {protocol}/{latency}ms -> ");

            //var stats = dao.GetStats(setId, protocol, latency);
            var stats = selector(setId, protocol, latency);

            Append(protocol, latency, stats, file, t => t.Mean, entryHeaders);
            Append(protocol, latency, stats, file, t => t.StandardDeviation, entryHeaders);
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

        private static void Append(string protocol, int latency, Dictionary<string, StatEntry> stats, InvertedCSVFile file, Func<StatEntry, object> selector, string[] entryHeaders)
        {
            var fileEntryErr = new List<string>();

            fileEntryErr.Add($"{protocol} ({latency} ms)");
            fileEntryErr.Add(latency.ToString());

            foreach (var testName in entryHeaders)
            {
                fileEntryErr.Add(selector(stats[testName]).ToString());
            }

            file.AddList(fileEntryErr);
        }
    }
}
