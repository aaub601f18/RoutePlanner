using System;
using System.Collections.Generic;
using System.Diagnostics;

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

            while (q.Count != 0)
            {
                var u = ExtractMinDist(ref q);
                s.Add(u);
                //Console.WriteLine("On " + u.Id);
                foreach (var neighbour in u.edges)
                {
                    int alt = u.Distance + Data.GetDistance(u, neighbour.DestV, rangeStart, rangeEnd);
                    if (alt < neighbour.DestV.Distance)
                    {
                        //Console.WriteLine("Relaxed " + neighbour.DestV.Id);
                        neighbour.DestV.Distance = alt;
                        neighbour.DestV.Prev = u;
                    }
                }
            }

            return s;
        }

        public static List<Vertex> BuildGraph(List<Edge> edges)
        {
            List<Vertex> graph = new List<Vertex>();

            foreach (var edge in edges)
            {
                edge.StartV.edges.Add(edge);    
            }
            
            foreach (var edge in edges)
            {
                if (!graph.Contains(edge.StartV))
                {
                    graph.Add(edge.StartV);
                }

                if (!graph.Contains(edge.DestV))
                {
                    graph.Add(edge.DestV);
                }
            }

            return graph;
        }

        public static Vertex ExtractMinDist(ref List<Vertex> vertices)
        {
            int min = Int32.MaxValue;
            Vertex minV = null; // ¯\_(ツ)_/¯ 
            int numberOfVertices = vertices.Count;
            
            foreach (var vertex in vertices)
            {
                if (vertex.Distance <= min)
                {
                    min = vertex.Distance;
                    minV = vertex;
                }
            }
            
            vertices.Remove(minV); // pop
            return minV;
        }

    }
}