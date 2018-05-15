using System;
using System.Collections.Generic;
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
        public LinkedList<Edge> neighbours;

        public Vertex()
        {
            neighbours = new LinkedList<Edge>();
        }

        public Vertex(string id) : this()
        {
            Id = id;
        }
        
        public Vertex(string id, string lat, string lng) : this()
        {
            Id = id;
            Lat = lat;
            Lng = lng;
        }
    }
}