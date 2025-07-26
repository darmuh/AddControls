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
			actionNames = Config.Bind("Help", "Possible Action Names", "", "This will list all possible action names separated by a comma.\nEditing this will do nothing!");
			actionNames.Value = "";
			InputSystem.actions.Do(action => actionNames.Value += $"{action.name}, ");
			actionNames.Value = actionNames.Value.Substring(0, actionNames.Value.Length - 2);

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

			ConfigEntry<string> settingChanged = (ConfigEntry<string>)settingChangedArg.ChangedSetting;

			if (settingChanged == null)
				return;

			Log.LogDebug($"CONFIG SETTING CHANGE EVENT - {settingChangedArg.ChangedSetting.Definition.Key}");

			if (settingChanged == bindsAdded || settingChanged == bindsRemoved)
				BindManager.Start();
		}
	}
}