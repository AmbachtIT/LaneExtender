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
    public class ToolButton : UIButton
    {


        public void OnGUI()
        {
            if (!UIView.HasModalInput() && !UIView.HasInputFocus())
            {
                if (Tool.ToggleKey.IsPressed(Event.current))
                {
                    SimulateClick();
                }
            }
        }

        protected override void OnClick(UIMouseEventParameter p)
        {
            if (p.buttons.IsFlagSet(UIMouseButton.Left) && Tool.Instance != null)
            {
                Singleton<ToolManager>.instance.m_properties.CurrentTool = Tool.Instance;
            }
        }
    }
}
