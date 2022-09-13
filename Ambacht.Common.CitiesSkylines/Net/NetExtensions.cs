using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ambacht.Common.CitiesSkylines.Mathmatics;
using ColossalFramework;
using UnityEngine;

namespace Ambacht.Common.CitiesSkylines.Net
{
    public static class NetExtensions
    {

        public static ushort SetNetType(this ushort segmentId, NetInfo info)
        {
            var segment = segmentId.GetSegment();
            segmentId.RemoveSegment();
            info.CreateSegment(
                segment.m_startNode,
                segment.m_endNode,
                segment.m_startDirection,
                segment.m_endDirection,
                segment.IsInvert(),
                out segmentId);
            return segmentId;
        }


        public static void ReverseAndInvert(this ref ushort segmentId)
        {
            var segment = segmentId.GetSegment();
            segmentId.RemoveSegment();
            segment.Info.CreateSegment(
                segment.m_endNode,
                segment.m_startNode,
                segment.m_endDirection,
                segment.m_startDirection,
                !segment.IsInvert(),
                out segmentId);
        }

        public static void RemoveNode(this ushort nodeId) =>
            Singleton<NetManager>.instance.ReleaseNode(nodeId);


        public static void RemoveSegment(this ushort segmentId, bool keepNodes = true) =>
            Singleton<NetManager>.instance.ReleaseSegment(segmentId, keepNodes);


        public static bool CreateSegment(this NetInfo info, ushort startNodeId, ushort endNodeId, Vector3 startDir, Vector3 endDir, bool invert, out ushort newSegmentId) => 
            Singleton<NetManager>.instance.CreateSegment(out newSegmentId, ref Singleton<SimulationManager>.instance.m_randomizer, info, startNodeId, endNodeId, startDir, endDir, Singleton<SimulationManager>.instance.m_currentBuildIndex, Singleton<SimulationManager>.instance.m_currentBuildIndex, invert);


        public static ref NetNode GetNode(this ushort nodeId) => ref NetManager.instance.m_nodes.m_buffer[nodeId];
        public static ref NetSegment GetSegment(this ushort segmentId) => ref NetManager.instance.m_segments.m_buffer[segmentId];

        public static bool IsInvert(ref this NetSegment segment) => (segment.m_flags & NetSegment.Flags.Invert) != 0;




        private static void PlayEffect(EffectInfo.SpawnArea spawnArea, bool create)
        {
            var effectInfo = create ? Singleton<NetManager>.instance.m_properties.m_placementEffect : Singleton<NetManager>.instance.m_properties.m_bulldozeEffect;
            Singleton<EffectManager>.instance.DispatchEffect(effectInfo, spawnArea, Vector3.zero, 0f, 1f, Singleton<AudioManager>.instance.DefaultGroup, 0u, avoidMultipleAudio: true);
        }


        private static void PlayEffect(BezierTrajectory trajectory, float halfWidth, bool create) => PlayEffect(new EffectInfo.SpawnArea(trajectory.Trajectory, halfWidth, 0f), create);
        public static void PlaySegmentEffect(this ushort segmentId, bool create) => PlayEffect(new EffectInfo.SpawnArea(new BezierTrajectory(segmentId).Trajectory, segmentId.GetSegment().Info.m_halfWidth, 0f), create);
        private static void PlayNodeEffect(ushort nodeId, bool create)
        {
            ref var node = ref nodeId.GetNode();
            PlayEffect(new EffectInfo.SpawnArea(node.m_position, Vector3.zero, node.Info.m_halfWidth), create);
        }
        /*private static void PlayEffect(Point[] points, float halfWidth, bool create)
        {
            for (var i = 1; i < points.Length; i += 1)
                PlayEffect(GetTrajectory(points[i - 1], points[i]), halfWidth, true);
        }
        private static void PlayAudio(bool create)
        {
            var effectInfo = create ? Singleton<NetManager>.instance.m_properties.m_placementEffect : Singleton<NetManager>.instance.m_properties.m_bulldozeEffect;
            Singleton<EffectManager>.instance.DispatchEffect(effectInfo, new EffectInfo.SpawnArea(), Vector3.zero, 0f, 1f, Singleton<AudioManager>.instance.DefaultGroup, 0u, avoidMultipleAudio: true);
        }

        private static BezierTrajectory GetTrajectory(Point first, Point second) => new BezierTrajectory(first.Position, first.ForwardDirection, second.Position, second.BackwardDirection);*/

    }
}
