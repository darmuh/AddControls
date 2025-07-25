using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine.InputSystem;

namespace AddControls
{
    [BepInAutoPlugin]
    public partial class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance { get; private set; } = null!;
        internal static ManualLogSource Log { get; private set; } = null!;
        internal static ConfigEntry<string> bindsAdded = null!;
        internal static ConfigEntry<string> bindsRemoved = null!;
        internal static ConfigEntry<string> actionNames = null!;
        internal static ConfigEntry<Options> Reload = null!;

        internal enum Options
        {
            Reload,
            ReloadNow
        }

        private void Start()
        {
            Instance = this;
            Log = Logger;
            Log.LogInfo($"Plugin {Name} is loaded!");

            bindsAdded = Config.Bind("Binds", "Binds Added", "", "List your custom binds here.\nFormat is ActionName:binding;ActionName2:binding2");
            bindsRemoved = Config.Bind("Binds", "Binds Removed", "", "List default bindings you wish to remove.\nFormat is ActionName:binding;ActionName2:binding2");
            Reload = Config.Bind<Options>("Binds", "Reload Settings", Options.Reload, "Change this setting in-game if you wish to reload your bind settings!");
            Reload.Value = Options.Reload;
            actionNames = Config.Bind("Help", "Possible Action Names", "", "This will list all possible action names separated by a comma.\nEditing this will do nothing!");
            actionNames.Value = "";
            InputSystem.actions.Do(a => actionNames.Value += $"{a.name}, ");

            BindManager.Start();

            Config.SettingChanged += OnSettingChanged;
            Config.ConfigReloaded += OnConfigReloaded;
        }

        private void OnConfigReloaded(object sender, EventArgs e)
        {
            Log.LogDebug("Config has been reloaded!");
            BindManager.Start();
        }

        private void OnSettingChanged(object sender, SettingChangedEventArgs settingChangedArg)
        {
            if (settingChangedArg.ChangedSetting == null)
                return;

            if (settingChangedArg.ChangedSetting.BoxedValue.GetType() == typeof(Options))
            {
                ConfigEntry<Options> settingChanged = (ConfigEntry<Options>)settingChangedArg.ChangedSetting;

                if (settingChanged == null)
                    return;

                Log.LogDebug($"CONFIG SETTING CHANGE EVENT - {settingChangedArg.ChangedSetting.Definition.Key}");

                if (settingChanged == Reload)
                {
                    BindManager.Start();
                }
            }
        }
    }
}
