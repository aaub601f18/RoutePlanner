using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;

namespace RoutePlanner
{
    public static class Test
    {
        public static Dictionary<string, int> BuildGraphCount = new Dictionary<string, int>();
        public static Dictionary<string, int> OptGraphCount = new Dictionary<string, int>();
        public static Dictionary<string, int> UpdateDistanceCount = new Dictionary<string, int>();
        public static int NumberOfVertices { get; set; }
        public static int NumberOfEdges { get; set; }
        public static int NumberOfTimeRecords { get; set; }
        public static TimeSpan BuildGraphTimeDifference { get; set; }
        public static TimeSpan UpdateDistanceTimeDifference { get; set; }
        public static TimeSpan OptimizingRouteTimeDifference { get; set; }
        

        public static int GetTotalCount()
        {
            return (BuildGraphCount.Values.Sum() + OptGraphCount.Values.Sum() + UpdateDistanceCount.Values.Sum());
        }

        public static void Prep()
        {
            for (int i = 1; i <= 6; i++)
            {
                UpdateDistanceCount.Add(i.ToString(), 0);
            }

            for (int i = 1; i <= 15; i++)
            {
                OptGraphCount.Add(i.ToString(), 0);
            }

            for (int i = 1; i <= 11; i++)
            {
                BuildGraphCount.Add(i.ToString(), 0);
            }
        }

        public static void PrintResult()
        {
            Console.WriteLine("\n-- Build Graph Test Result:\n\n");
            foreach (var c in BuildGraphCount)
            {
                Console.WriteLine(c.Key + " : " + c.Value);
            }

            Console.WriteLine("Total: " + BuildGraphCount.Values.Sum());

            Console.WriteLine("\n-- Optimal Route Test Result:");
            foreach (var c in OptGraphCount)
            {
                Console.WriteLine(c.Key + " : " + c.Value);
            }

            Console.WriteLine("Total: " + OptGraphCount.Values.Sum());

            Console.WriteLine("\n-- Update Distance Test Result:");
            foreach (var c in UpdateDistanceCount)
            {
                Console.WriteLine(c.Key + " : " + c.Value);
            }

            Console.WriteLine("Total: " + UpdateDistanceCount.Values.Sum());
        }

        public static void SaveResult()
        {
            List<string> strs = new List<string>();
            if (System.IO.File.Exists(@"testresult.txt"))
            {
                System.IO.File.Delete(@"testresult.txt");
            }
            strs.Add("Test Result");
            strs.Add("--------------------------");
            strs.Add("Number of vertices: " + NumberOfVertices);
            strs.Add("Number of edges: " + NumberOfEdges);
            strs.Add("Number of time records: " + NumberOfTimeRecords);
            strs.Add("--------------------------");
            strs.Add("Build graph time: " + BuildGraphTimeDifference.TotalSeconds + " s");
            strs.Add("Updating distance time: " + UpdateDistanceTimeDifference.TotalSeconds + " s");
            strs.Add("Optimizing route time: " + OptimizingRouteTimeDifference.TotalSeconds + " s");
            strs.Add("--------------------------");
            strs.Add("-- Build Graph Test Result");
            strs.Add("--------------------------");
            foreach (var c in BuildGraphCount)
            {
                strs.Add(c.Key + " : " + c.Value);
            }

            strs.Add("Total: " + BuildGraphCount.Values.Sum());
            strs.Add("-----------------------------");
            strs.Add("-- Optimal Route Test Result:");
            strs.Add("-----------------------------");
            foreach (var c in OptGraphCount)
            {
                strs.Add(c.Key + " : " + c.Value);
            }

            strs.Add("Total: " + OptGraphCount.Values.Sum());
            strs.Add("-------------------------------");
            strs.Add("-- Update Distance Test Result:");
            strs.Add("-------------------------------");
            foreach (var c in UpdateDistanceCount)
            {
                strs.Add(c.Key + " : " + c.Value);
            }

            strs.Add("Total: " + UpdateDistanceCount.Values.Sum());
            System.IO.File.AppendAllLines(@"testresult.txt", strs);
        }
    }
}