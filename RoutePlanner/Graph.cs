using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Timers;

namespace RoutePlanner
{
    public class Graph
    {
        public static List<Vertex> OptRoute(List<Vertex> graph, Vertex source)
        {
            Test.OptGraphCount["1"]++;
            List<Vertex> q = new List<Vertex>();
            Test.OptGraphCount["2"]++;
            List<Vertex> s = new List<Vertex>();

            foreach (var v in graph) // Init
            {
                Test.OptGraphCount["3"]++;
                if (source.Id == v.Id)
                {
                    Test.OptGraphCount["4"]++;
                    v.Distance = 0;
                }
                else
                {
                    Test.OptGraphCount["5"]++;
                    v.Distance = int.MaxValue;
                }

                Test.OptGraphCount["6"]++;
                q.Add(v);
            }

            while (q.Count > 0)
            {
                Test.OptGraphCount["7"]++;
                // Extract MinDist (Min dist will come in front of q, thus at index 0. 
                q.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                Test.OptGraphCount["8"]++;
                var u = q[0];
                Test.OptGraphCount["9"]++;
                q.RemoveAt(0);
                foreach (var neighbour in u.neighbours)
                {
                    Test.OptGraphCount["11"]++;
                    int dist = neighbour.AvgTravelTime;
                    //int dist = Data.GetDistance(neighbour, rangeStart, rangeEnd); // Get distance from database.
                    Test.OptGraphCount["12"]++;
                    int alt = u.Distance + dist;


                    if (alt < neighbour.DestV.Distance)
                    {
                        Test.OptGraphCount["13"]++;
                        neighbour.DestV.Distance = alt;
                        Test.OptGraphCount["14"]++;
                        neighbour.DestV.Prev = u;
                    }
                }

                Test.OptGraphCount["10"]++;
                s.Add(u);
            }

            Test.OptGraphCount["15"]++;
            return s;
        }

        public static bool Validate(List<TimeRecord> records, string SourceId)
        {
            foreach (var record in records)
            {
                if (record.Edge.StartV.Id == SourceId)
                {
                    //Console.WriteLine("Vertex is in Graph and is a source of a path");
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

        public static List<Vertex> GetRoute(Vertex source, Vertex destination, DateTime startTime, DateTime endTime)
        {
            // Building graph
            DateTime buildiStart = DateTime.Now;
            var graph = Data.BuildGraph(source, destination);
            DateTime buildingEnd = DateTime.Now;
            var buildDifference = buildingEnd - buildiStart;
            Console.WriteLine("Time for building graph: " + buildDifference.TotalMilliseconds + " ms | " +
                              buildDifference.TotalSeconds + " s");

            // Updating average time
            DateTime updatingStart = DateTime.Now;
            Data.UpdateDistance(ref graph, startTime, endTime);
            DateTime updatingEnd = DateTime.Now;
            var updatingDifference = updatingEnd - updatingStart;
            Console.WriteLine("Time for updating average travel times: " + updatingDifference.TotalMilliseconds + " ms | " + updatingDifference.TotalSeconds + " s");

            // Optimizing route
            DateTime optRouteStart = DateTime.Now;
            var optRoute = OptRoute(graph, source);
            DateTime optRouteEnd = DateTime.Now;
            var optRouteDifference = optRouteEnd - optRouteStart;
            Console.WriteLine("Time for opt graph: " + optRouteDifference.TotalMilliseconds + " ms | " +
                              optRouteDifference.TotalSeconds + " s");

            Test.BuildGraphTimeDifference = buildDifference;
            Test.UpdateDistanceTimeDifference = updatingDifference;
            Test.OptimizingRouteTimeDifference = optRouteDifference;
            
            return optRoute;
        }
    }
}