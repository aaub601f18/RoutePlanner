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
            
            //Data.PopulateTimeRecords(DateTime.Today, edges);
            string lat = "57.031746";
            string lng = "10.306410";

            var source = Data.GetVertex(lat, lng);
            Console.WriteLine("Source Id: " + source.Id);
            
            DateTime startRange = new DateTime(2018, 05, 09, 16, 00, 00);
            DateTime endRange = new DateTime(2018, 05, 09, 16, 15, 00); 
            
            var edges =  Data.GetLiveData(startRange, endRange, "57.031100", "", "57.031278", "");
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
                var optRoute = Graph.OptRoute(graph, source, startRange, endRange);    
            }
            
            //Console.ReadKey();
            
            Data.Close();
        }
    }
}