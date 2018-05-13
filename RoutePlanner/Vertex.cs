using System;
using System.Numerics;

namespace RoutePlanner
{
    public class Vertex
    {
        public string Id { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public Boolean Null { get; set; }
        public int Distance { get; set; }
        public Vertex Prev { get; set; }
        
        public Vertex(){}
        
        public Vertex(string id, string lat, string lng)
        {
            Id = id;
            Lat = lat;
            Lng = lng;
        }
    }
}