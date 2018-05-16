using System;

namespace RoutePlanner
{
    public class Edge
    {
        public string Id { get; set; }
        public Vertex StartV { get; set; }
        public Vertex DestV { get; set; }
        public bool Oneway { get; set; }
        public string Speed { get; set; }
        public string Distance { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int AvgTravelTime { get; set; }
        
        public Edge(string id, Vertex sv, Vertex dv, string oneway, string speed)
        {
            Id = id;
            StartV = sv;
            DestV = dv;
            Speed = speed;

            if (oneway == "N")
            {
                Oneway = false;
            }
            else
            {
                Oneway = true;
            }
            
        }
    }
    

}