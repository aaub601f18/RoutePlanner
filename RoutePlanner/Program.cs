using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace RoutePlanner
{
    class Program
    {
        static void Main(string[] args)
        {
            Data.Open();

/*
            var edges = Data.GetEdges();
            DateTime d1 = DateTime.Now;
            Console.WriteLine("press");
            Console.ReadKey();
            Data.PopulateTimeRecords(d1, edges);
  */


            //Data.PopulateTimeRecords(DateTime.Today, edges);
            string sourceLat = "57.031651";
            string sourceLng = "9.969195";

            var source = Data.GetVertex("1032373169");
            Console.WriteLine("Source Id: " + source.Id);

            DateTime startRange = new DateTime(2018, 05, 09, 16, 00, 00);
            DateTime endRange = new DateTime(2018, 05, 09, 16, 15, 00);

            var edges = Data.GetLiveData(startRange, endRange, "57.029707", "", "57.039139", "");
            bool checker = false;

            Console.WriteLine("Press key to check whether graph is possible..");
            Console.ReadKey();

            foreach (var edge in edges)
            {
                if (edge.StartV.Id == source.Id)
                {
                    checker = true;
                }
            }

            if (checker == false)
            {
                Console.WriteLine("Graph is not possible.");
            }
            else
            {
                Console.WriteLine("Graph is possible.");
            }

            Console.WriteLine("Press key to run graph and opt..");
            Console.ReadKey();


            if (checker)
            {
                var graph = Graph.BuildGraph(edges);

                foreach (var vertex in graph)
                {
                    Console.WriteLine(vertex.Id + "    " + vertex.neighbours.Count);
                }

                var optRoute = Graph.OptRoute(graph, source, startRange, endRange);

                foreach (var el in optRoute)
                {
                    if (el.Prev != null)
                    {
                        Console.WriteLine("Vertex: " + el.Id + "\nDistance: " + el.Distance + "\nPrevious: " +
                                          el.Prev.Id + "\n");
                    }
                    else
                    {
                        Console.WriteLine("Vertex: " + el.Id + "\nDistance: " + el.Distance + "\nPrevious: " + "null" +
                                          "\n");
                    }
                }
            }

            Data.Close();
        }
    }
}