using System;
using System.Collections.Generic;

namespace RoutePlanner
{
    public class TimeRecord
    {
        public DateTime Date { get; set; }
        public int TimeTravelledInSeconds { get; set; }
        public Edge Edge { get; set; }

        public TimeRecord(DateTime date, int timeTravelledInSeconds, Edge edge)
        {
            Date = date;
            TimeTravelledInSeconds = timeTravelledInSeconds;
            Edge = edge;
        }


        public static List<TimeRecord> GenerateTimeRecords(DateTime date, List<Edge> edges, Random seed,
            int maxEntries = 20)
        {
            List<TimeRecord> timeRecords = new List<TimeRecord>();

            foreach (var edge in edges)
            {
                int numberOfEntries = seed.Next(maxEntries);

                for (int i = 0; i < numberOfEntries; i++)
                {
                    DateTime d = new DateTime(date.Year, date.Month, date.Day, seed.Next(24), seed.Next(60),
                        seed.Next(60));
                    timeRecords.Add(new TimeRecord(d, seed.Next(10, 120), edge));
                }
            }

            return timeRecords;
        }
    }
}