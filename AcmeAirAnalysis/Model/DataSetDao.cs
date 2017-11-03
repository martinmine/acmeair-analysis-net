using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AcmeAirAnalysis.Model
{
    class DataSetDao
    {
        private MySqlConnection conn;

        public DataSetDao(string connString)
        {
            conn = new MySqlConnection(connString);
            conn.Open();
        }

        public DataSet GetByDate(string date)
        {
            string sql = "SELECT id, date FROM import WHERE date=@date";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@date", date);

            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    return new DataSet
                    {
                        Id = (int)rdr["id"],
                        Date = (string)rdr["date"]
                    };
                }
            }

            return null;
        }

        public IEnumerable<Network>GetNetworkRecords(int importId, string protocol, int latency)
        {
            string sql = "SELECT idnetworkResponse, importId, protocol, latency, servicename, interface, txBytes, txPackets, rxBytes, rxPackets " +
                "FROM network " +
                "WHERE importId=@importid AND protocol=@protocol AND latency=@latency";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@importid", importId);
            cmd.Parameters.AddWithValue("@protocol", protocol);
            cmd.Parameters.AddWithValue("@latency", latency);

            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    yield return new Network
                    {
                        Id = (int)rdr["idnetworkResponse"],
                        ImportId = (int)rdr["importId"],
                        Protocol = (string)rdr["protocol"],
                        Latency = (int)rdr["latency"],
                        Servicename = (string)rdr["servicename"],
                        Interface = (string)rdr["interface"],
                        TxBytes = (int)rdr["txBytes"],
                        TxPackets = (int)rdr["txPackets"],
                        RxBytes = (int)rdr["rxBytes"],
                        RxPackets = (int)rdr["rxPackets"]
                    };
                }
            }
        }

        public IEnumerable<Request> GetRequestRecords(int importId, string protocol, int latency)
        {
            string sql = "SELECT id, importId, protocol, latency, timestamp, responseTime, testName, responseCode, responseLength " +
                "FROM request " +
                "WHERE importId=@importid AND protocol=@protocol AND latency=@latency";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@importid", importId);
            cmd.Parameters.AddWithValue("@protocol", protocol);
            cmd.Parameters.AddWithValue("@latency", latency);

            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    yield return new Request
                    {
                        Id = (int)rdr["id"],
                        ImportId = (int)rdr["importId"],
                        Protocol = (string)rdr["protocol"],
                        Latency = (int)rdr["latency"],
                        Timestamp = (string)rdr["timestamp"],
                        ResponseTime = (int)rdr["responseTime"],
                        TestName = (string)rdr["testName"],
                        ResponseCode = (string)rdr["responseCode"],
                        ResponseLength = (int)rdr["responseLength"],
                    };
                }
            }
        }

        public Dictionary<string, StatEntry> GetStats(int importId, string protocol, int latency)
        {
            string sql = "SELECT AVG(responseTime) AS mean, MAX(responseTime) as max, MIN(responseTime) as min, STD(responseTime) as stdDev, testName " +
                    "FROM request " +
                    "WHERE importId = @importId AND protocol = @protocol AND latency = @lat " +
                    "GROUP BY testName";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@importId", importId);
            cmd.Parameters.AddWithValue("@protocol", protocol);
            cmd.Parameters.AddWithValue("@lat", latency);

            Dictionary<string, StatEntry> averages = new Dictionary<string, StatEntry>();

            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    averages.Add((string)rdr["testName"], new StatEntry
                    {
                        Max = (int)rdr["max"],
                        Min = (int)rdr["min"],
                        Mean = (decimal)rdr["mean"],
                        StandardDeviation = (double)rdr["stdDev"]
                    });
                }
            }

            return averages;
        }
    }
}
