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

            fileWriter.WriteToFile("test.csv");
        }

        private static void DoRequestAnalysis(int latency, string protocol, int setId, DataSetDao dao, InvertedCSVFile file)
        {
            Console.Write($"Analyzing requests for protocol {protocol}/{latency}ms -> ");

            var stats = dao.GetStats(setId, protocol, latency);
            var fileEntry = new List<string>();

            fileEntry.Add(protocol);
            fileEntry.Add(latency.ToString());

            foreach (var testName in testNames)
            {
                fileEntry.Add(stats[testName].Mean.ToString());
            }

            file.AddList(fileEntry);

            Console.WriteLine($"DONE!");
        }
    }   
}
