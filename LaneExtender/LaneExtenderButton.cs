using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using ColossalFramework.Plugins;
using UnityEngine;
using UnityEngine.UI;

namespace LaneExtender
{
    public class LaneExtenderButton : UIButton
    {


        public void OnGUI()
        {
            if (!UIView.HasModalInput() && !UIView.HasInputFocus())
            {
                if (LaneExtenderTool.ToggleKey.IsPressed(Event.current))
                {
                    SimulateClick();
                }
            }
        }

        protected override void OnClick(UIMouseEventParameter p)
        {
            if (p.buttons.IsFlagSet(UIMouseButton.Left) && LaneExtenderTool.Instance != null)
            {
                Singleton<ToolManager>.instance.m_properties.CurrentTool = LaneExtenderTool.Instance;
            }
        }
    }
}
