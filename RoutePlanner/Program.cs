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


            #region Generate Timerecords

/*
            var edges = Data.GetEdges();
            DateTime d1 = DateTime.Now;
            Console.WriteLine("press");
            Console.ReadKey();
            Data.PopulateTimeRecords(d1, edges);
            Data.PopulateTimeRecords(DateTime.Today, edges);
 */

            #endregion


            string sourceLat = "57.031651";
            string sourceLng = "9.969195";

            var source = Data.GetVertex("1032373169");
            Console.WriteLine("Source Id: " + source.Id);

            DateTime startRange = new DateTime(2018, 05, 09, 16, 00, 00);
            DateTime endRange = new DateTime(2018, 05, 09, 16, 15, 00);

            var records = Data.GetLiveData(startRange, endRange, "57.029707", "", "57.039139", "");


            if (Graph.ValidateInput(records, source.Id))
            {
                var graph = Graph.BuildGraph(records);
                var optRoute = Graph.OptRoute(graph, source, startRange, endRange);

                if (System.IO.File.Exists(@"output.txt"))
                {
                    System.IO.File.Delete(@"output.txt");
                }

                foreach (var el in optRoute)
                {
                    List<string> strings = new List<string>();

                    strings.Add("Vertex: " + el.Id);
                    strings.Add("Distance: " + el.Distance);

                    if (el.Prev != null)
                    {
                        strings.Add("Previous: " + el.Prev.Id);
                    }
                    else
                    {
                        strings.Add("Previous: null");
                    }

                    strings.Add("--------------");

                    System.IO.File.AppendAllLines(@"output.txt", strings);
                }
            }
            else
            {
                Console.WriteLine("Graph cannot be validated.");
            }

            Data.Close();
        }
    }
}