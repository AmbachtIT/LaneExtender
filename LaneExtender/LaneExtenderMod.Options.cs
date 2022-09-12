using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework.UI;
using ICities;

namespace LaneExtender
{
    public partial class LaneExtenderMod
    {

        public void OnSettingsUI(UIHelperBase helper)
        {
            var group = (UIHelper)helper.AddGroup(Name);
            var panel = (UIPanel) group.self;
            panel.gameObject.AddComponent<KeyBindingControl>();
        }

    }
}
