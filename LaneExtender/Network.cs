using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LaneExtender
{
    public class Network
    {

        public string Name { get; set; }

        public float LaneWidth { get; set; }

        public List<RoadType> RoadTypes { get; set; } = new List<RoadType>();

        public static Network Default = new Network()
        {
            Name = "Vanilla highways",
            LaneWidth = 4,
            RoadTypes = new List<RoadType>()
            {
                new RoadType() {Name = "HighwayRamp", LaneCount = 1},
                new RoadType() {Name = "Two Lane Highway", LaneCount = 2},
                new RoadType() {Name = "Highway", LaneCount = 3},
                new RoadType() {Name = "Four Lane Highway", LaneCount = 4}
            }
        };

        public static Network GetNetwork(string name)
        {
            foreach (var network in AllNetworks())
            {
                if (network.GetRoad(name) != null)
                {
                    return network;
                }
            }

            return null;
        }



        public RoadType GetRoad(string name)
        {
            return RoadTypes.SingleOrDefault(r => r.Name == name);
        }

        public RoadType GetRoad(int laneCount)
        {
            return RoadTypes.SingleOrDefault(r => r.LaneCount == laneCount);
        }


        public static IEnumerable<Network> AllNetworks()
        {
            yield return Default;
        }

    }
}
