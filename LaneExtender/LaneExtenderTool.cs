using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ModsCommon.Utilities;
using UnityEngine;

namespace LaneExtender
{
    public class LaneExtenderTool : ToolBase
    {

        public static readonly SavedInputKey ToggleKey = new SavedInputKey("toggleTool", LaneExtenderMod.SettingsFileName, SavedInputKey.Encode(KeyCode.E, true, false, false), true);

        public static LaneExtenderTool Instance { get; private set; }
        private ToolController _toolController;
        private LaneExtenderButton _button;


        public LaneExtenderTool()
        {
            Instance = this;
        }

        protected override void Awake()
        {
            _toolController = FindObjectOfType<ToolController>();
            enabled = false;
            _button = (LaneExtenderButton) UIView.GetAView().AddUIComponent(typeof(LaneExtenderButton));
        }

        public void Uninstall()
        {
            Destroy(this);
            Instance = null;
        }


        protected override void OnEnable()
        {

        }

        protected override void OnDisable()
        {

        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            var data = new OverlayData(cameraInfo);
        }
    }
}
