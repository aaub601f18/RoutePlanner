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
                    v.Distance = 99999;
                }
                
                q.Add(v);
            }

            while (q.Count != 0)
            {
                var u = ExtractMinDist(ref q);
                s.Add(u);
                Console.WriteLine("On " + u.Id);
                foreach (var neighbour in u.neighbours)
                {
                    Console.WriteLine("\tNeighbour " + neighbour.DestV.Id);
                    int alt = u.Distance + Data.GetDistance(u, neighbour.DestV, rangeStart, rangeEnd);
                    if (alt < neighbour.DestV.Distance)
                    {
                        Console.WriteLine("Relaxed " + neighbour.DestV.Id);
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
            int c = 0;
            foreach (var edge in edges) // Init
            {
                if (!graph.Contains(edge.StartV))
                {
                    graph.Add(edge.StartV);
                    c++;
                }

                if (!graph.Contains(edge.DestV))
                {
                    graph.Add(edge.DestV);
                    c++;
                }
            }

            foreach (var edge in edges)
            {
                foreach (var vertex in graph)
                {
                    if (edge.StartV.Id == vertex.Id)
                    {
                        vertex.neighbours.AddLast(edge);
                    }
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