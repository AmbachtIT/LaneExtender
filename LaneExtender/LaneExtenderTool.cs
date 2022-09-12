using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ModsCommon.Utilities;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace LaneExtender
{
    public class LaneExtenderTool : ToolBase
    {

        public static readonly SavedInputKey ToggleKey = new SavedInputKey("toggleTool", LaneExtenderMod.SettingsFileName, SavedInputKey.Encode(KeyCode.E, true, false, false), true);

        public static LaneExtenderTool Instance { get; private set; }
        private LaneExtenderButton _button;
        private ushort _hoveringId = 0;


        public LaneExtenderTool()
        {
            Instance = this;
        }

        protected override void Awake()
        {
            if (_button == null)
            {
                _button = (LaneExtenderButton)UIView.GetAView().AddUIComponent(typeof(LaneExtenderButton));
            }
        }

        public void Uninstall()
        {
            Destroy(this);
            Instance = null;
        }


        protected override void OnEnable()
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, $"LaneExtenderTool: Enabled");
        }

        protected override void OnDisable()
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, $"LaneExtenderTool: Disabled");
        }

        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

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
        }


        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (_hoveringId != 0)
            {
                var buffer = NetManager.instance?.m_segments?.m_buffer;
                if (buffer == null)
                {
                    return;
                }
                ref var segment = ref buffer[_hoveringId];
                if (segment.Info != null)
                {
                    NetTool.RenderOverlay(cameraInfo, ref segment, _hoverColor, _hoverColor);
                }
            }
        }

        private readonly Color _hoverColor = new Color32(0, 181, 255, 255);
    }
}
