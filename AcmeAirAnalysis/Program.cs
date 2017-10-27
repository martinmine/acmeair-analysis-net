using AcmeAirAnalysis.Model;
using MySql.Data.MySqlClient;
using System;

namespace AcmeAirAnalysis
{
    class Program
    {
        private static MySqlConnection conn;

        static void Main(string[] args)
        {
            DataSetDao dao = new DataSetDao("server=localhost;user=root;database=acmeair;port=3306;password=123;SslMode=none");

            DataSet set = dao.GetByDate("2017-10-27");

            int req = 0, net = 0;

            var g = dao.GetNetworkRecords(set.Id, "coap", 0);
            var u = dao.GetRequestRecords(set.Id, "coap", 0);

            foreach (var a in g) { req++; }
            foreach (var a in u) { net++; }

            Console.WriteLine($"Got {net} net records, {req} requests");
            Console.ReadLine();
        }
    }
}
