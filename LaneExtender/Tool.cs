using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ambacht.Common.CitiesSkylines;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using Epic.OnlineServices.Presence;
using ModsCommon;
using ModsCommon.Utilities;
using NodeController;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace LaneExtender
{
    public class Tool : ToolBase
    {

        public static readonly SavedInputKey ToggleKey = new SavedInputKey("toggleTool", Mod.SettingsFileName, SavedInputKey.Encode(KeyCode.E, true, false, false), true);

        public static Tool Instance { get; private set; }
        private ToolButton _button;
        private ushort _hoveringId = 0;
        private bool _waitingForMouseButtonUp = false;


        public Tool()
        {
            Instance = this;
        }

        protected override void Awake()
        {
            if (_button == null)
            {
                _button = (ToolButton)UIView.GetAView().AddUIComponent(typeof(ToolButton));
            }

            enabled = false;
        }

        public void Uninstall()
        {
            Destroy(this);
            Instance = null;
        }


        protected override void OnEnable()
        {
            _log.Info($"LaneExtenderTool: Enabled");
            enabled = true;

        }

        protected override void OnDisable()
        {
            _log.Info($"LaneExtenderTool: Disabled");
        }

        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            if (!enabled)
            {
                return;
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var input = new RaycastInput(ray, Camera.main.farClipPlane)
            {
                m_ignoreTerrain = false,
                m_ignoreSegmentFlags = NetSegment.Flags.None,
                m_ignoreNodeFlags = NetNode.Flags.All,
                m_ignorePropFlags = PropInstance.Flags.All,
                m_ignoreTreeFlags = TreeInstance.Flags.All,
                m_ignoreBuildingFlags = Building.Flags.All,

                m_ignoreCitizenFlags = CitizenInstance.Flags.All,
                m_ignoreVehicleFlags = Vehicle.Flags.Created,
                m_ignoreDisasterFlags = DisasterData.Flags.All,
                m_ignoreTransportFlags = TransportLine.Flags.All,
                m_ignoreParkedVehicleFlags = VehicleParked.Flags.All,
                m_ignoreParkFlags = DistrictPark.Flags.All,
            };
            RayCast(input, out var output);
            _hoveringId = output.m_netSegment;

            if (_waitingForMouseButtonUp)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    _waitingForMouseButtonUp = false;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (_hoveringId != 0)
                    {
                        Perform(ref GetHoveringSegment());
                    }
                    _waitingForMouseButtonUp = true;
                }
            }

        }

        private void Perform(ref NetSegment segment)
        {
            var info = segment.Info;
            _log.Info($"Clicked on segment: {info.name}");
            LogNodeData("start", segment.m_startNode);
            LogNodeData("end", segment.m_endNode);

        }

        private void LogNodeData(string prefix, ushort nodeId)
        {
            var node = NetManager.instance.m_nodes.m_buffer[nodeId];
            _log.Info($"{prefix} node id    : {nodeId}");
            _log.Info($"{prefix} node flags : {node.m_flags}");
            var manager = SingletonManager<Manager>.Instance;
            if (!manager.GetNodeData(nodeId, out var nodeData))
            {
                _log.Info($"{prefix} node has no node controller data");
            }
            else
            {
                _log.Info($"{prefix} node controller style: {nodeData.Style}");
                foreach (var segmentId in node.SegmentIds())
                {
                    if (!manager.GetSegmentData(nodeId, segmentId, out var segmentData))
                    {
                        _log.Info($"{prefix} node segment {segmentId} has no node controller data");
                    }
                    else
                    {
                        _log.LogFields(segmentData, $"{prefix} node segment {segmentId}", true);
                    }
                }
            }
        }



        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (_hoveringId != 0)
            {
                ref var segment = ref GetHoveringSegment();
                var color = GetColor(segment);
                NetTool.RenderOverlay(cameraInfo, ref segment, color, color);
            }
        }

        private Color GetColor(NetSegment segment)
        {
            var info = segment.Info;
            var road = Network.GetRoad(info.name);
            if (road == null)
            {
                return Color.red;
            }

            var roadPlusOne = Network.GetRoad(road, road.LaneCount + 1);
            if (roadPlusOne == null)
            {
                return Color.yellow;
            }

            return _colorBlue;
        }

        private ref NetSegment GetHoveringSegment()
        {
            return ref NetManager.instance.m_segments.m_buffer[_hoveringId];
        }

        private readonly Log _log = Services.Log;

        private readonly Color _colorBlue = new Color32(0, 181, 255, 255);

    }

}
