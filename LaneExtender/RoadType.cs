﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaneExtender
{
    public class RoadType
    {
        public string Name { get; set; }
        public int LaneCount { get; set; }

        public bool IsEnabled { get; private set; }

        public void Enable() => IsEnabled = true;

    }
}
