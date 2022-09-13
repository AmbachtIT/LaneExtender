using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaneExtender
{
    public class NodeCreator
    {


        public void DoIt()
        {
            // AbsoluteAngle: a.AbsoluteAngle - b.AbsoluteAngle = PI
            // Weight: 33, 39 ?????
            // bool IsMainRoad: Yes
            // bool IsRoad: Yes
            // bool IsTunnel: No
            // bool IsTrack: No
            // bool IsPath: No
            // VehicleTwist, TwistAngle: 0
            // _offsetValue: 9.995
            // _rotateValue: a.RotateValue = -b.RotateValue, if there is a curve. 0 on a straight segment
            // Shift: 0 or (+/-)1.625
            // Stretch: 1
            // Position: delta: (17.3, 0,  10.2), (15.1, 0, 13.3). Lengths: 20.08, 20.12
            // Direction: a.Direction = -b.Direction

        }

    }
}
