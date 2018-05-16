using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace RoutePlanner
{
    public class Graph
    {
        public static List<Vertex> OptRoute(List<Vertex> graph, Vertex source, DateTime rangeStart, DateTime rangeEnd)
        {
            List<Vertex> q = new List<Vertex>();
            List<Vertex> s = new List<Vertex>();

            foreach (var v in graph) // Init
            {
                if (source.Id == v.Id)
                {
                    v.Distance = 0;
                }
                else
                {
                    v.Distance = 10000;
                }

                q.Add(v);
            }

            while (q.Count > 0)
            {
                // Extract MinDist (Min dist will come in front of q, thus at index 0. 
                q.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                var u = q[0];
                q.RemoveAt(0);
                foreach (var neighbour in u.neighbours)
                {
                    int dist = neighbour.AvgTravelTime;
                    int alt = u.Distance + dist;

                    //int alt = u.Distance - Data.GetDistance(neighbour, rangeStart, rangeEnd); // Get distance from database. 
                    Console.WriteLine(alt +"<" + neighbour.DestV.Distance);
                    if (alt < neighbour.DestV.Distance)
                    {
                        neighbour.DestV.Distance = alt;
                        neighbour.DestV.Prev = u;
                    }
                }

                s.Add(u);
            }

            return s;
        }

        public static List<Vertex> BuildGraph(List<TimeRecord> records)
        {
            List<Vertex> graph = new List<Vertex>();

            List<Edge> edges = new List<Edge>();

            foreach (var record in records)
            {
                int currentEdgeTotalTravelTime = 0;
                int currentEdgeNumberOfRecords = 0;

                var edge = record.Edge;
                if (!edges.Any(x => x.Id == record.Edge.Id))
                {
                    foreach (var re in records)
                    {
                        if (re.Edge.Id == edge.Id)
                        {
                            currentEdgeTotalTravelTime += re.TimeTravelledInSeconds;
                            currentEdgeNumberOfRecords++;
                        }
                    }

                    int avg = currentEdgeTotalTravelTime / currentEdgeNumberOfRecords;
                    edge.AvgTravelTime = avg;
                    edges.Add(edge);
                }
            }

            foreach (var record in records)
            {
                var u = record.Edge.StartV;
                var v = record.Edge.DestV;

                foreach (var edge in edges)
                {
                    if (edge.StartV.Id == u.Id)
                    {
                        u.neighbours.AddLast(edge);
                    }

                    if (edge.DestV.Id == u.Id && edge.Oneway == false)
                    {
                        v.neighbours.AddLast(new Edge(v, u, "N", edge.Speed));
                    }
                }

                if (!graph.Any(x => x.Id == u.Id))
                {
                    graph.Add(u);
                }

                if (!graph.Any(x => x.Id == v.Id))
                {
                    graph.Add(v);
                }
            }

            return graph;
        }

        public static Vertex ExtractMinDist(ref List<Vertex> vertices)
        {
            vertices.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            var v = vertices[0];
            Console.WriteLine("popped " + v.Id);
            vertices.RemoveAt(0);
            return v;
        }

        public static bool Validate(List<TimeRecord> records, string SourceId)
        {
            foreach (var record in records)
            {
                if (record.Edge.StartV.Id == SourceId)
                {
                    Console.WriteLine("Vertex is in Graph and is a source of a path");
                    return true;
                }

                if (record.Edge.DestV.Id == SourceId)
                {
                    Console.WriteLine("Vertex is in Graph but is not a source of a path");
                    return false;
                }
            }

            Console.WriteLine("Vertex is not in Graph");
            return false;
        }

        public static List<Vertex> GetRoute(Vertex source, Vertex destination, DateTime startDate, DateTime endDate)
        {
            var records = Data.GetLiveData(startDate, endDate, source, destination);
            List<Vertex> optRoute;
            if (Validate(records, source.Id))
            {
                var graph = BuildGraph(records);
                optRoute = OptRoute(graph, source, startDate, endDate);
            }
            else
            {
                throw new ArgumentException();
            }

            return optRoute;
        }
    }
}