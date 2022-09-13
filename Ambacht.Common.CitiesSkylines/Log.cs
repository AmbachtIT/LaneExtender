using ColossalFramework.Plugins;
using Epic.OnlineServices.Presence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Ambacht.Common.CitiesSkylines
{
    public class Log
    {

        private readonly string _prefix;

        public Log(string prefix)
        {
            _prefix = prefix;
        }

        public void Info(string message)
        {
            message = Prefix(message);
#if DEBUG
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, message);
#endif
            Debug.Log(message);
        }

        private string Prefix(string message) => $"[{_prefix}] {message}";

        public void LogFields(object item, string prefix, bool omitDefaultValues = false)
        {
            var type = item.GetType();
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance))
            {
                var value = field.GetValue(item);
                if (omitDefaultValues)
                {
                    var typeField = field.GetType();
                    var valueDefault = GetDefaultValue(typeField);
                    if (object.Equals(value, valueDefault))
                    {
                        continue;
                    }
                }
                Info($"{prefix}.{field.Name} = {value}");
            }
        }

        private object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

    }
}
