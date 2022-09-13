using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaneExtender
{
    public class RoadType
    {
        public string Name { get; set; }
        public int LaneCount { get; set; }

        public bool IsEnabled => Info != null;

        public NetInfo Info { get; private set; }

        public void SetInfo(NetInfo info)
        {
            if (Info != null)
            {
                throw new InvalidOperationException($"Net info has already been set");
            }
            this.Info = info;
        }
    }
}
