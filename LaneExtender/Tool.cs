using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ambacht.Common.CitiesSkylines;
using Ambacht.Common.CitiesSkylines.Net;
using Ambacht.Common.CitiesSkylines.Rendering;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using Epic.OnlineServices.Presence;
using ModsCommon;
using NodeController;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Networking.Types;
using OverlayData = Ambacht.Common.CitiesSkylines.Rendering.OverlayData;

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
            enabled = false;
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
            if (_hoveringId != operation.SelectedId)
            {
                operation.Init(_hoveringId);
            }

            if (_waitingForMouseButtonUp)
            {
                if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
                {
                    _waitingForMouseButtonUp = false;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    operation.ChangeLanes(1);
                    _waitingForMouseButtonUp = true;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    operation.ChangeLanes(-1);
                    _waitingForMouseButtonUp = true;
                }
            }

        }

        private Operation operation = new Operation();

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            operation.RenderOverlay(cameraInfo);
        }

       
        private ref NetSegment GetHoveringSegment()
        {
            return ref NetManager.instance.m_segments.m_buffer[_hoveringId];
        }

        private readonly Log _log = Services.Log;

        private readonly Color _colorBlue = new Color32(0, 181, 255, 255);

    }

}
