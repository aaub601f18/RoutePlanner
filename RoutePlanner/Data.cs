using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
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

        public static List<Vertex> BuildGraph(Vertex start, Vertex destination)
        {
            Test.NumberOfEdges = 0;
            Test.BuildGraphCount["1"]++;
            List<Vertex> graph = new List<Vertex>();
            Test.BuildGraphCount["2"]++;
            string sql = String.Format(@"
                SELECT e.id as edgeId, 
                    s.id as startId, 
                    s.lat as startLat, 
                    s.lng as startLng, 
                    d.id as endId, 
                    d.lat as endLat, 
                    d.lng as endLng, 
                    e.speed, e.oneway
                FROM edge AS e
                    INNER JOIN vertex AS s
                        ON s.id=e.n1id
                    INNER JOIN vertex AS d
                        ON d.id=e.n2id
                    WHERE s.lat<={0}
                        AND s.lng>={1}
                        AND d.lat>={2} 
                        AND d.lng<={3}",
                start.Lat,
                start.Lng, destination.Lat, destination.Lng);
            
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader res = cmd.ExecuteReader();
            
            while (res.Read())
            {
                Test.BuildGraphCount["3"]++;
                Test.NumberOfEdges++;
                Test.BuildGraphCount["4"]++;
                var u = new Vertex(res[1].ToString(), res[2].ToString(), res[3].ToString());
                Test.BuildGraphCount["5"]++;
                var v = new Vertex(res[4].ToString(), res[5].ToString(), res[6].ToString());
                Test.BuildGraphCount["6"]++;
                var e = new Edge(res[0].ToString(), u, v, res[7].ToString(), res[8].ToString());

                Test.BuildGraphCount["9"]++;
                u.neighbours.AddLast(e);
                if (!e.Oneway)
                {
                    Test.BuildGraphCount["10"]++;
                    v.neighbours.AddLast(e);
                }
                                
                if (!graph.Any(x=>x.Id==u.Id))
                {
                    Test.BuildGraphCount["7"]++;
                    graph.Add(u);
                }

                if (!graph.Any(x=>x.Id==v.Id))
                {
                    Test.BuildGraphCount["8"]++;
                    graph.Add(v);
                }
            }

            res.Dispose();
            res.Close();
            Test.BuildGraphCount["11"]++;
            Test.NumberOfVertices = graph.Count;
            return graph;
        }

        public static void UpdateDistance(ref List<Vertex> graph, DateTime startTime, DateTime endTime)
        {
            Test.NumberOfTimeRecords = 0;        
            string sql = String.Format(@"
                SELECT edgeId AS eid, avg(t.timeTravelSeconds) AS avg
		        FROM timerecords AS t
			        INNER JOIN edge AS e ON e.id=t.edgeid
		        WHERE t.date BETWEEN '{0}' AND '{1}'
		        GROUP BY e.id", startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader res = cmd.ExecuteReader();
            Test.UpdateDistanceCount["1"]++;
            while (res.Read())
            {
                Test.NumberOfTimeRecords++;
                Test.UpdateDistanceCount["2"]++;

                foreach (var vertex in graph)
                {
                    Test.UpdateDistanceCount["3"]++;
                    foreach (var e in vertex.neighbours)
                    {
                        Test.UpdateDistanceCount["4"]++;
                        if (e.Id == res[0].ToString())
                        {
                            Test.UpdateDistanceCount["5"]++;
                            e.AvgTravelTime = Convert.ToInt32(res[1]);
                        }
                    }
                }
            }

            Test.UpdateDistanceCount["6"]++;
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