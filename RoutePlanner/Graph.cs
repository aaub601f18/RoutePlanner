using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

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
                    v.Distance = 99999;
                }

                q.Add(v);
            }

            while (q.Count > 0)
            {
                q.Sort((x, y) => x.Distance.CompareTo(y.Distance)); // Extract MinDist (Min dist will come in front of q, thus at index 0. 
                s.Add(q[0]);
                
                foreach (var neighbour in q[0].neighbours)
                {
                    //int alt = q[0].Distance + Data.GetDistance(q[0], neighbour.DestV, rangeStart, rangeEnd);
                    int alt = q[0].Distance + neighbour.AvgTravelTime; //Data.GetDistance(u, neighbour.DestV, rangeStart, rangeEnd);
                    if (alt < neighbour.DestV.Distance)
                    {
                        neighbour.DestV.Distance = alt;
                        neighbour.DestV.Prev = q[0];
                    }
                }
                q.RemoveAt(0);
            }

            return s;
        }

        public static List<Vertex> BuildGraph(List<TimeRecord> records)
        {
            List<Vertex> graph = new List<Vertex>();

            foreach (var record in records) // Init
            {
                if (!graph.Contains(record.Edge.StartV))
                {
                    graph.Add(record.Edge.StartV);
                }

                if (!graph.Contains(record.Edge.DestV))
                {
                    graph.Add(record.Edge.DestV);
                }
            }

            foreach (var record in records)
            {
                int currentEdgeTotalTravelTime = 0;
                int currentEdgeNumberOfRecords = 0;

                foreach (var edge in records)
                {
                    if (record.Edge.Id == edge.Edge.Id)
                    {
                        currentEdgeTotalTravelTime += edge.TimeTravelledInSeconds;
                        currentEdgeNumberOfRecords++;
                    }
                }

                record.Edge.AvgTravelTime = currentEdgeTotalTravelTime / currentEdgeNumberOfRecords;
            }

            foreach (var record in records)
            {
                foreach (var vertex in graph)
                {
                    if (record.Edge.StartV.Id == vertex.Id)
                    {
                        vertex.neighbours.AddLast(record.Edge);
                    }
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

        public static bool ValidateInput(List<TimeRecord> records, string SourceId)
        {
            foreach (var record in records)
            {
                if (record.Edge.StartV.Id == SourceId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}