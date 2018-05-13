using System;
using System.Collections.Generic;

namespace RoutePlanner
{
    class Program
    {
        static void Main(string[] args)
        {
            //Data.PopulateTimeRecords(DateTime.Today, edges);
            
            DateTime startRange = new DateTime(2018, 05, 09, 16, 00, 00);
            DateTime endRange = new DateTime(2018, 05, 09, 16, 15, 00); 
            var edges =  Data.GetLiveData(startRange, endRange, "57.031100", "", "57.031278", "");

            foreach (var edge in edges)
            {
                Console.WriteLine(edge.StartV.Id + " -- " + edge.Id + " -> " + edge.DestV.Id);
            }
            
            //Console.ReadKey();
        }
    }
}