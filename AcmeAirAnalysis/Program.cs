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

        static void Main(string[] args)
        {
            DataSetDao dao = new DataSetDao("server=localhost;user=root;database=acmeair;port=3306;password=123;SslMode=none");

            DataSet set = dao.GetByDate("2017-10-27");

            var fileWriter = new InvertedCSVFile();

            List<string> header = new List<string>();
            header.Add("Protocol");
            header.Add("Latency");
            header.AddRange(testNames.ToList());

            fileWriter.AddList(header);

            int[] latencies = { 0, 2, 5 };
            string[] protocols = { "coap", "h2c", "http1.1" };

            foreach (var latency in latencies)
            {
                foreach (var protocol in protocols)
                {
                    DoRequestAnalysis(latency, protocol, set.Id, dao, fileWriter);
                }
            }

            fileWriter.WriteToFile($"results-{DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss")}.csv");
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
