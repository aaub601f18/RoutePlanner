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

            var sourceId = "56697987"; // Get the id from https://www.openstreetmap.org
            var destinationId = "68469691"; // Get the id from https://www.openstreetmap.org

            var source = Data.GetVertex(sourceId);
            var destination = Data.GetVertex(destinationId);

            Console.WriteLine("Source Id: " + source.Id);
            Console.WriteLine("Destination Id: " + destination.Id);

            DateTime startRange = new DateTime(2018, 05, 16, 14, 00, 00);
            DateTime endRange = new DateTime(2018, 05, 16, 17, 30, 00);

            var optRoute = Graph.GetRoute(source, destination, startRange, endRange);

            if (System.IO.File.Exists(@"output.txt"))
            {
                System.IO.File.Delete(@"output.txt");
            }

            foreach (var el in optRoute)
            {
                List<string> strings = new List<string>();

                if (el.Prev == null && el.Distance == 0)
                {
                    strings.Add("Vertex: " + el.Id);
                    strings.Add("Distance: " + el.Distance);
                    strings.Add("Previous: null");
                    strings.Add("--------------");
                }

                if (el.Prev != null)
                {
                    strings.Add("Vertex: " + el.Id);
                    strings.Add("Distance: " + el.Distance);
                    strings.Add("Previous: " + el.Prev.Id);
                    strings.Add("--------------");
                }

                System.IO.File.AppendAllLines(@"output.txt", strings);
            }


            Data.Close();
        }
    }
}