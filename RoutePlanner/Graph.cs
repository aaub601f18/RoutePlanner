using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

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
                    v.Distance = Int32.MaxValue;
                }

                q.Add(v);
            }

            while (q.Count > 0)
            {
                q.Sort((x, y) => x.Distance.CompareTo(y.Distance)); // Extract MinDist (Min dist will come in front of q, thus at index 0. 
                var u = q[0];
                q.RemoveAt(0);
                foreach (var neighbour in u.neighbours)
                {
                    //int alt = q[0].Distance + Data.GetDistance(q[0], neighbour.DestV, rangeStart, rangeEnd); // Get distance from database. 
                    int alt = u.Distance + neighbour.AvgTravelTime; //Data.GetDistance(u, neighbour.DestV, rangeStart, rangeEnd);
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
                
                record.Edge.StartV.neighbours.AddLast(record.Edge);
                if (record.Edge.Oneway)
                {
                    record.Edge.DestV.neighbours.AddLast(record.Edge);
                }
                
                if (!graph.Any(x => x.Id == record.Edge.StartV.Id))
                {
                    graph.Add(record.Edge.StartV);
                }

                if (!graph.Any(x => x.Id == record.Edge.DestV.Id))
                {
                    graph.Add(record.Edge.DestV);
                }
                
                

            }
/*
            

            foreach (var record in records) // To get the average travel time into memory instead of having to extract it from DB for each iteration in OptRoute. This is more effective although it does not take data added after extract into account.
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
            }*/

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