using ColossalFramework.Plugins;
using Epic.OnlineServices.Presence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaneExtender
{
    public class Logger
    {

        public void Info(string message)
        {
#if DEBUG
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, message);
#endif

        }

    }
}
