using ColossalFramework.Plugins;
using Epic.OnlineServices.Presence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LaneExtender
{
    public class Logger
    {

        public void Info(string message)
        {
            message = Prefix(message);
#if DEBUG
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, message);
#endif
            Debug.Log(message);
        }


        private string Prefix(string message) => $"[LaneExtender] {message}";

    }
}
