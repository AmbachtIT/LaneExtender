using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


        public static RoadType GetRoad(string name)
        {
            return AllRoads().SingleOrDefault(r => r.Name == name);
        }

        public static RoadType GetRoad(RoadType type, int laneCount)
        {
            foreach (var network in AllNetworks())
            {
                if (network.RoadTypes.Contains(type))
                {
                    return network.RoadTypes.SingleOrDefault(r => r.LaneCount == laneCount);
                }
            }

            return null;
        }


        public static IEnumerable<Network> AllNetworks()
        {
            yield return Default;
        }

        public static IEnumerable<RoadType> AllRoads()
        {
            return AllNetworks().SelectMany(r => r.RoadTypes);
        }

    }
}
