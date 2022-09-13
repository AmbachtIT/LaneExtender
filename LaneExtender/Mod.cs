using System;
using System.Text;
using Ambacht.Common.CitiesSkylines;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;

namespace LaneExtender
{
    public partial class Mod : LoadingExtensionBase, IUserMod
    {
        public string Name => "Lane Extender";

        public string Description => "Allows you to extend freeway lanes easily without messing up alignment of the roads.";

        public const string SettingsFileName = "LaneExtender";


        public override void OnCreated(ILoading loading)
        {

        }


        public override void OnLevelLoaded(LoadMode mode)
        {
            if (ShouldInstallForGameMode(mode))
            {
                InstallMod();
            }
        }

        private bool ShouldInstallForGameMode(LoadMode mode)
        {
            return mode == LoadMode.NewMap
                   || mode == LoadMode.NewGame
                   || mode == LoadMode.NewGameFromScenario
                   || mode == LoadMode.LoadMap
                   || mode == LoadMode.LoadGame
                   || mode == LoadMode.LoadScenario;
        }


        public override void OnLevelUnloading()
        {
            UnInstallMod();
        }

        public void OnEnabled()
        {
            if (LoadingManager.exists && LoadingManager.instance.m_loadingComplete)
            {
                InstallMod();
            }
        }

        public void OnDisabled()
        {
            if (LoadingManager.exists && LoadingManager.instance.m_loadingComplete)
            {
                UnInstallMod();
            }
        }

        public static void InstallMod()
        {
            if (Tool.Instance == null)
            {
                ToolsModifierControl.toolController.gameObject.AddComponent<Tool>();

                var builder = new StringBuilder();
                builder.AppendLine();
                builder.AppendLine("-------------------------------");
                builder.AppendLine("- Checking roads              -");
                builder.AppendLine("-------------------------------");
                for (uint i = 0; i < PrefabCollection<NetInfo>.PrefabCount(); i++)
                {
                    var info = PrefabCollection<NetInfo>.GetPrefab(i);
                    if (info != null)
                    {
                        // This is a road prefab
                        var road = Network.GetRoad(info.name);
                        if (road != null)
                        {
                            road.SetInfo(info);
                            builder.AppendLine($"{info.name}: {road.LaneCount} lanes");
                        }
                    }
                }
                builder.AppendLine("-------------------------------");
                _log.Info(builder.ToString());
            }
        }

        public static void UnInstallMod()
        {
            Tool.Instance?.Uninstall();
        }

        private static readonly Log _log = Services.Log;

    }
}
