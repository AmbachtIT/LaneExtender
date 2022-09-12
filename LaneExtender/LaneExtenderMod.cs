using System;
using ICities;
using UnityEngine;

namespace LaneExtender
{
    public partial class LaneExtenderMod : LoadingExtensionBase, IUserMod
    {
        public string Name => "Lane Extender";

        public string Description => "Allows you to extend freeway lanes easily without messing up alignment of the roads.";

        public const string SettingsFileName = "LaneExtender";


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
            if (LaneExtenderTool.Instance == null)
            {
                ToolsModifierControl.toolController.gameObject.AddComponent<LaneExtenderTool>();
            }
        }

        public static void UnInstallMod()
        {
            LaneExtenderTool.Instance?.Uninstall();
        }

    }
}
