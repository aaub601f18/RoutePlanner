using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Schema;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace RoutePlanner
{
    public class Data
    {
        private static MySqlConnection conn;

        private static string connStr =
            "server=127.0.0.1;user=root;database=mickpeder_bachelor;port=3306;password=P@ssw0rd";

        public static void Open()
        {
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Mysql connection error:");
                Console.WriteLine(e.ToString());
            }
        }

        public static void Close()
        {
            conn.Close();
        }

        //public static Edge GetEdge(string id) {} //TODO: Implement (just copy/paste/edit from GetVertex

        public static Vertex GetVertex(string id)
        {
            try
            {
                string sql = "SELECT * FROM vertex WHERE id=" + id;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader res = cmd.ExecuteReader();

                res.Read();
                Vertex v = new Vertex(res[0].ToString(), res[1].ToString(), res[2].ToString());

                res.Dispose();
                res.Close();
                return v;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static Vertex GetVertex(string lat, string lng)
        {
            try
            {
                string sql = String.Format("SELECT * FROM vertex WHERE lat={0} AND lng={1}", lat, lng);
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader res = cmd.ExecuteReader();

                res.Read();
                Vertex v = new Vertex(res[0].ToString(), res[1].ToString(), res[2].ToString());

                res.Dispose();
                res.Close();
                return v;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void PopulateTimeRecords(DateTime date, List<Edge> edges)
        {
            List<TimeRecord> records = TimeRecord.GenerateTimeRecords(date, edges, new Random(), 20);

            string query = "INSERT INTO timerecords (date, timeTravelSeconds, edgeId) VALUES ";
            StringBuilder str = new StringBuilder();
            str.Append(query);
            int count = records.Count;

            foreach (var record in records)
            {
                Console.Clear();
                Console.WriteLine(count + " left.");
                string sql =
                    String.Format("INSERT INTO timerecords (date, timeTravelSeconds, edgeId) VALUES ('{0}', {1}, {2})",
                        record.Date.ToString("yyyy-MM-dd HH:mm:ss"), record.TimeTravelledInSeconds, record.Edge.Id);
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                count -= 1;
            }
        }

        public static List<Edge> GetEdges()
        {
            List<Edge> edges = new List<Edge>();
            string sql =
                "select edge.id, edge.n1id, edge.n2id, edge.oneway, edge.speed from edge inner join vertex as n1 on edge.n1id=n1.id inner join vertex as n2 on edge.n2id=n2.id where n1.lat<=57.039139 and n2.lat<=57.039139 and n1.lat>=57.029707 and n2.lat>=57.029707";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader res = cmd.ExecuteReader();

            while (res.Read())
            {
                edges.Add(new Edge(res[0].ToString(), new Vertex(res[1].ToString()), new Vertex(res[2].ToString()),
                    res[3].ToString(), res[4].ToString()));
            }

            res.Dispose();
            res.Close();
            return edges;
        }


        public static List<Vertex> GetVertices(double startLat, double destinationLat)
        {
            List<Vertex> vertices = new List<Vertex>();

            string sql = "";
            if (startLat >= destinationLat) // fra vest til øst
            {
                sql = String.Format("SELECT * FROM vertex WHERE lat<={0} AND lat>={1}", startLat, destinationLat);
            }
            else
            {
                sql = String.Format("SELECT * FROM vertex WHERE lat>={0} AND lat<={1}", startLat, destinationLat);
            }

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader res = cmd.ExecuteReader();

            while (res.Read())
            {
                vertices.Add(new Vertex(res[0].ToString(), res[1].ToString(), res[2].ToString()));
            }

            res.Dispose();
            res.Close();

            return vertices;
        }

        public static int GetDistance(Vertex u, Vertex v, DateTime rangeStart, DateTime rangeEnd)
        {
            string sql = String.Format(@"
                SELECT avg(t.timeTravelSeconds) as avg
                FROM timerecords AS t
                    INNER JOIN edge AS e
                        ON e.id=t.edgeid
                    INNER JOIN vertex AS s
                        ON s.id=e.n1id
                    INNER JOIN vertex AS d
                        ON d.id=e.n2id
                WHERE s.id={0}
                    AND d.id={1} 
                    AND t.date BETWEEN '{2}' 
                        AND '{3}';", u.Id, v.Id, rangeStart.ToString("yyyy-MM-dd HH:mm:ss"),
                rangeEnd.ToString("yyyy-MM-dd HH:mm:ss")
            );

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader res = cmd.ExecuteReader();

            int avg = int.MaxValue;
            while (res.Read())
            {
                if (!(res[0] is DBNull))
                {
                    avg = Convert.ToInt32(res[0]);
                }
            }

            res.Dispose();
            res.Close();

            return avg;
        }

        public static List<TimeRecord> GetLiveData(DateTime rangeStart, DateTime rangeEnd, Vertex start,
            Vertex destination)
        {
            List<TimeRecord> records = new List<TimeRecord>();
            string sql = String.Format(@"
                SELECT e.id as edgeId, 
                    s.id as startId, 
                    s.lat as startLat, 
                    s.lng as startLng, 
                    d.id as endId, 
                    d.lat as endLat, 
                    d.lng as endLng, 
                    e.speed, e.oneway, t.timeTravelSeconds as traveltime, t.date as date
                FROM timerecords AS t
                    INNER JOIN edge AS e
                        ON e.id=t.edgeId
                    INNER JOIN vertex AS s
                        ON s.id=e.n1id
                    INNER JOIN vertex AS d
                        ON d.id=e.n2id
                    WHERE 
                        t.date BETWEEN '{0}'
                            AND '{1}'
                        AND s.lat<={2}
                        AND s.lng>={3}
                        AND d.lat>={4} 
                        AND d.lng<={5}",
                rangeStart.ToString("yyyy-MM-dd HH:mm:ss"), rangeEnd.ToString("yyyy-MM-dd HH:mm:ss"), start.Lat,
                start.Lng, destination.Lat, destination.Lng);

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader res = cmd.ExecuteReader();

            while (res.Read())
            {
                Vertex startV = new Vertex(res[1].ToString(), res[2].ToString(), res[3].ToString());
                Vertex destV = new Vertex(res[4].ToString(), res[5].ToString(), res[6].ToString());
                Edge edge = new Edge(res[0].ToString(), startV, destV, res[7].ToString(), res[8].ToString());
                TimeRecord record = new TimeRecord(DateTime.Parse(res[10].ToString()),
                    Convert.ToInt32(res[9].ToString()), edge);
                records.Add(record);
            }

            res.Dispose();
            res.Close();
            return records;
        }

        public static List<TimeRecord> GetLiveData(DateTime rangeStart, DateTime rangeEnd, string startLat,
            string startLng,
            string destinationLat, string destinationLng)
        {
            List<TimeRecord> records = new List<TimeRecord>();
            string sql = String.Format(@"
                SELECT e.id as edgeId, 
                    s.id as startId, 
                    s.lat as startLat, 
                    s.lng as startLng, 
                    d.id as endId, 
                    d.lat as endLat, 
                    d.lng as endLng, 
                    e.speed, e.oneway, t.timeTravelSeconds as traveltime, t.date as date
                FROM timerecords AS t
                    INNER JOIN edge AS e
                        ON e.id=t.edgeId
                    INNER JOIN vertex AS s
                        ON s.id=e.n1id
                    INNER JOIN vertex AS d
                        ON d.id=e.n2id
                    WHERE 
                        t.date BETWEEN '{0}'
                            AND '{1}'
                        AND IF({2}>={3}, 
                            (s.lat<={2} 
                                AND s.lng>={4}
                                AND d.lat>={3} 
                                AND d.lng<={5}), 
                            (s.lat>={2} 
                                AND s.lng<={4}
                                AND d.lat<={3} 
                                AND d.lng>={5}))",
                rangeStart.ToString("yyyy-MM-dd HH:mm:ss"), rangeEnd.ToString("yyyy-MM-dd HH:mm:ss"), startLat,
                destinationLat, startLng, destinationLng
            );

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader res = cmd.ExecuteReader();

            while (res.Read())
            {
                Vertex startV = new Vertex(res[1].ToString(), res[2].ToString(), res[3].ToString());
                Vertex destV = new Vertex(res[4].ToString(), res[5].ToString(), res[6].ToString());
                Edge edge = new Edge(res[0].ToString(), startV, destV, res[7].ToString(), res[8].ToString());
                TimeRecord record = new TimeRecord(DateTime.Parse(res[10].ToString()),
                    Convert.ToInt32(res[9].ToString()), edge);
                records.Add(record);
            }

            res.Dispose();
            res.Close();
            return records;
        }
    }
}