using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace RoutePlanner
{
    public class Data
    {
        private static MySqlConnection conn;
        private static string connStr = "server=127.0.0.1;user=root;database=mickpeder_bachelor;port=3306;password=P@ssw0rd";
        
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
   /*     
        public static Edge GetEdge(string id)
        {
            try
            {
                string sql = "SELECT * FROM edge WHERE id=" + id;
                var dat = new Data();
                MySqlCommand cmd = new MySqlCommand(sql, dat.conn);
                MySqlDataReader res = cmd.ExecuteReader();
            
                res.Read();

                var v1 = GetVertex(res[1].ToString());
                var v2 = GetVertex(res[2].ToString());
                        
                Edge e = new Edge(res[0].ToString(), res[1].ToString(), res[2].ToString(), res[3].ToString(), res[4].ToString(), res[5].ToString(), res[6].ToString(), res[7].ToString());
                dat.Close();
                return e;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
 */ 
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
        
        public static List<Edge> GetEdgeByRelation(string id)
        {
            try
            {
                List<Edge> result = new List<Edge>();
                string sql = String.Format("SELECT * FROM edge WHERE n1id={0} OR n2id={0}", id);
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader res = cmd.ExecuteReader();

                while (res.Read())
                {
                   // result.Add(new Edge(res[0].ToString(), res[1].ToString(), res[2].ToString(), res[3].ToString(), res[4].ToString(), res[5].ToString(), res[6].ToString(), res[7].ToString()));                    
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void PopulateTimeRecords(DateTime date, List<Edge> edges)
        {
            List<TimeRecord> records = TimeRecord.GenerateTimeRecords(date, edges, new Random(), 1000);
            
            foreach (var record in records)
            {
                string sql = String.Format("INSERT INTO timerecords (date, timeTravelSeconds, edgeId) VALUES ('{0}', {1}, {2})", record.Date.ToString("yyyy-MM-dd HH:mm:ss"), record.TimeTravelledInSeconds, record.Edge.Id);
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Edge> GetLiveData(DateTime rangeStart, DateTime rangeEnd, string startLat, string startLng,
            string destinationLat, string destinationLng)
        {
            List<Edge> edges = new List<Edge>();
            string sql = String.Format(@"
                SELECT e.id as edgeId, 
                    s.id as startId, 
                    s.lat as startLat, 
                    s.lng as startLng, 
                    d.id as endId, 
                    d.lat as endLat, 
                    d.lng as endLng, 
                    e.speed, e.oneway
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
                            AND d.lat>={3}), 
                            (s.lat>={2}
                            AND d.lat<={3}));",
                rangeStart.ToString("yyyy-MM-dd HH:mm:ss"), rangeEnd.ToString("yyyy-MM-dd HH:mm:ss"), startLat, destinationLat
            );

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader res = cmd.ExecuteReader();
            
            while (res.Read())
            {
                Vertex startV = new Vertex(res[1].ToString(), res[2].ToString(), res[3].ToString());
                Vertex destV = new Vertex(res[4].ToString(), res[5].ToString(), res[6].ToString());
                Edge edge = new Edge(res[0].ToString(), startV, destV, res[7].ToString(), res[8].ToString());
                
                edges.Add(edge);
            }
     
            res.Dispose();
            res.Close();
            return edges;
        }
/*
        public static List<Edge> GetEdges(List<Vertex> vertices)
        {
            var dat = new Data();
            List<Edge> edges = new List<Edge>();
            string sql = "SELECT * FROM edge";
            MySqlCommand cmd = new MySqlCommand(sql, dat.conn);
            MySqlDataReader res = cmd.ExecuteReader();

            res.Read();

            foreach (var vertex in vertices)
            {
                var edge = GetEdgeByRelation(vertex.id);
                edges.Add(edge);



            }
            
         
            
            
/* Langsom af h til
            while (res.Read())
            {
                var v1id = res[1].ToString();
                var v2id = res[2].ToString();

                Vertex v1 = new Vertex();
                Vertex v2 = new Vertex();
                
                foreach (var vertex in vertices)
                {
                    if (vertex.id == v1id)
                    {
                        v1 = vertex;
                        Console.WriteLine("added " + vertex.id + " to v1 of " + res[0].ToString());
                    }
                    else if (vertex.id == v2id)
                    {
                        v2 = vertex;
                        Console.WriteLine("added " + vertex.id + " to v2 of " + res[0].ToString());
                    }
                }
                
                edges.Add(new Edge(res[0].ToString(), v1, v2, res[3].ToString(), res[4].ToString(), res[5].ToString(), res[6].ToString(), res[7].ToString()));
            }


            dat.Close();
            return edges;
        } 
           
        public static List<Vertex> GetVertices(double startLat, double destinationLat)
        {
            var dat = new Data();
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

            MySqlCommand cmd = new MySqlCommand(sql, dat.conn);
            MySqlDataReader res = cmd.ExecuteReader();

            while (res.Read())
            {
                vertices.Add(new Vertex(res[0].ToString(),Convert.ToDouble(res[1]), Convert.ToDouble(res[2])));
            }

            dat.Close();
            return vertices;
        } 
        */
        
        
        public static int GetDistance(Vertex u, Vertex v, DateTime rangeStart, DateTime rangeEnd)
        {
            string sql = String.Format(@"
                SELECT sum(t.timeTravelSeconds), count(t.id)
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
                        AND '{3}';", u.Id, u.Id, rangeStart.ToString("yyyy-MM-dd HH:mm:ss"), rangeEnd.ToString("yyyy-MM-dd HH:mm:ss")
            );
            
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader res = cmd.ExecuteReader();

            int avg = int.MaxValue;
            while (res.Read())
            {
                if (!(res[0] is DBNull) && !(res[1] is DBNull))
                {
                    avg = Convert.ToInt32(res[0]) / Convert.ToInt32(res[1]);
                }
            }
            
            res.Dispose();
            res.Close();
            
            return avg;
        }
    }
}