using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ambacht.Common.CitiesSkylines.Net;
using Ambacht.Common.CitiesSkylines.Rendering;
using JetBrains.Annotations;
using ModsCommon;
using NodeController;
using UnityEngine;
using static NetInfo;

namespace LaneExtender
{
    public class Operation
    {

        public Operation()
        {
            Init(0);
        }

        public void Init(ushort segmentId)
        {
            Error = null;
            if (segmentId == 0)
            {
                Error = "No segment selected";
                return;
            }

            SelectedId = segmentId;
            Selected = SelectedId.GetSegment();
            SelectedNetwork = Network.GetNetwork(Selected.Info.name);
            SelectedRoadType = SelectedNetwork?.GetRoad(Selected.Info.name);
            if (SelectedRoadType == null || SelectedNetwork == null)
            {
                Error = "Selected segment network is not supported";
                return;
            }

            NodeId1 = Selected.m_startNode;
            Node1 = NodeId1.GetNode();
            NodeId2 = Selected.m_endNode;
            Node2 = NodeId2.GetNode();

            if (GetOtherSegment(Node1, out var other))
            {
                Segment1 = other;
                SegmentRoadType1 = SelectedNetwork.GetRoad(Segment1.Info.name);
            }
            else
            {
                return;
            }
            if (GetOtherSegment(Node2, out other))
            {
                Segment2 = other;
                SegmentRoadType2 = SelectedNetwork.GetRoad(Segment2.Info.name);
            }
            else
            {
                return;
            }

            if (SegmentRoadType1 == null || SegmentRoadType2 == null)
            {
                Error = "Adjacent road types must be of the same network type as the selected segment";
                return;
            }
        }


        private bool GetOtherSegment(NetNode node, out NetSegment other)
        {
            var segments = node.SegmentIds().Where(s => s != SelectedId).ToList();
            if (segments.Count == 1)
            {
                other = segments.Single().GetSegment();
                return true;
            }

            Error = "Unexpected number of segments at intersection";
            other = new NetSegment();
            return false;
        }

        public ushort SelectedId { get; private set; }

        public Network SelectedNetwork { get; private set; }

        public RoadType SelectedRoadType { get; private set; }

        public RoadType UpgradedRoadType { get; private set; }

        public NetSegment Selected { get; private set; }

        public ushort NodeId1 { get; private set; }
        public NetNode Node1 { get; private set; }
        public ushort NodeId2 { get; private set; }
        public NetNode Node2 { get; private set; }

        public NetSegment Segment1 { get; private set; }

        public RoadType SegmentRoadType1 { get; private set; }


        public NetSegment Segment2 { get; private set; }

        public RoadType SegmentRoadType2 { get; private set; }


        public void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (SelectedId == 0)
            {
                return;
            }
            var selected = this.Selected;
            var color = IsError ? Colors.Red : Colors.Blue;
            NetTool.RenderOverlay(cameraInfo, ref selected, color, color);

            RenderSegmentEnd(cameraInfo, selected.m_startNode);
            RenderSegmentEnd(cameraInfo, selected.m_endNode);
        }

        private void RenderSegmentEnd(RenderManager.CameraInfo cameraInfo, ushort nodeId)
        {
            var overlay = new OverlayData(cameraInfo)
            {
                Color = Color.white
            };
            var node = NetManager.instance.m_nodes.m_buffer[nodeId];
            node.m_position.RenderCircle(overlay, 10f);
            var manager = SingletonManager<Manager>.Instance;
            if (manager.GetNodeData(nodeId, out var nodeData))
            {
                foreach (var segmentId in node.SegmentIds())
                {
                    if (manager.GetSegmentData(nodeId, segmentId, out var segmentData))
                    {
                        segmentData.Position.RenderCircle(overlay, 5);
                    }
                }
            }
        }

        public string Error { get; private set; }

        public bool IsError => Error != null;

        public bool ChangeLanes(int deltaLanes)
        {
            if (IsError)
            {
                return false;
            }
            UpgradedRoadType = SelectedNetwork.GetRoad(SelectedRoadType.LaneCount + deltaLanes);
            if (UpgradedRoadType == null || !UpgradedRoadType.IsEnabled)
            {
                return false;
            }

            SimulationManager.instance.AddAction(() =>
            {
                SelectedId = SelectedId.SetNetType(UpgradedRoadType.Info);
                SelectedId.PlaySegmentEffect(true);

                AlignLeft(NodeId1, SegmentRoadType1);
                AlignLeft(NodeId2, SegmentRoadType2);

                Invalidate();
            });

            return true;
        }


        /// <summary>
        /// Aligns the segment so its left most lane aligns with the other end
        /// </summary>
        private void AlignLeft(ushort nodeId, RoadType roadType)
        {
            var data = NodeController[nodeId, true];

            
            if (UpgradedRoadType.LaneCount == roadType.LaneCount)
            {
                data.Type = NodeStyleType.Middle;
                data.Offset = 0;
            }
            else
            {
                data.Type = NodeStyleType.Custom;
                data.Offset = 20;
            }
            data.UpdateNode();
            //data.UpdateSegmentEnds();

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


        private void Invalidate()
        {
            SelectedId = 0;
        }


        private Manager NodeController => SingletonManager<Manager>.Instance;
    }
}
