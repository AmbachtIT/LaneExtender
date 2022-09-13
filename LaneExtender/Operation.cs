using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ambacht.Common.CitiesSkylines.Net;
using Ambacht.Common.CitiesSkylines.Rendering;
using ModsCommon;
using NodeController;
using UnityEngine;
using static NetInfo;

namespace LaneExtender
{
    public class Operation
    {

        public Operation(ushort segmentId)
        {
            if (segmentId == 0)
            {
                Error = "No segment selected";
                return;
            }

            SelectedId = segmentId;
            Selected = SelectedId.GetSegment();
            SelectedNetwork = LaneExtender.Network.GetNetwork(Selected.Info.name);
            SelectedRoadType = SelectedNetwork?.GetRoad(Selected.Info.name);
            if (SelectedRoadType == null)
            {
                Error = "Selected segment network is not supported";
                return;
            }

            Node1 = Selected.m_startNode.GetNode();
            Node2 = Selected.m_endNode.GetNode();
            
            if (GetOtherSegment(Node1, out var other))
            {
                Segment1 = other;
            }
            else
            {
                return;
            }
            if (GetOtherSegment(Node2, out other))
            {
                Segment2 = other;
            }
            else
            {
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

        public NetSegment Selected { get; private set; }
        
        public NetNode Node1 { get; private set; }
        public NetNode Node2 { get; private set; }

        public NetSegment Segment1 { get; private set; }
        public NetSegment Segment2 { get; private set; }


        public void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
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
            var roadUpgraded = SelectedNetwork.GetRoad(SelectedRoadType.LaneCount + deltaLanes);
            if (roadUpgraded == null || !roadUpgraded.IsEnabled)
            {
                return false;
            }

            SimulationManager.instance.AddAction(() =>
            {
                SelectedId = SelectedId.SetNetType(roadUpgraded.Info);
                SelectedId.PlaySegmentEffect(true);
            });
            Invalidate();
            return true;
        }

        private void Invalidate()
        {
            SelectedId = 0;
        }
    }
}
