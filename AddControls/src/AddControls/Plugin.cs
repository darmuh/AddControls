using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

namespace AddControls;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static Plugin Instance { get; private set; } = null!;
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static List<string> DeviceNames = [];
    internal static List<string> ValidKeyPaths = [];
    internal static ConfigEntry<string> BindsAdded = null!;

    private void Start()
    {
        Instance = this;
        Log = Logger;
        Log.LogInfo($"Plugin {Name} is loaded!\nConfiguration file is located at {Paths.ConfigPath}");

        BindsAdded = Config.Bind("Manual Binds", "Binds Added", "", "List your custom additional binds here.\nFormat is ActionName:binding;ActionName2:binding2\nNOTE: Any change to this setting will remove and refresh all additive binds!\nNOTE2: Find valid action names and keypath bindings in the LogOutput.log file when enabling bepinex's debug logging");

        ValidKeyPaths = GenerateValidKeyPaths();

        Log.LogMessage("Enable debug logging to see ALL valid ActionNames!");

        InputSystem.actions.Do(a =>
        {
            Log.LogDebug($"ActionName: " + a.name); //for custombindings

            if (a.bindings.Count > 1)
            {
                for (int i = 0; i < a.bindings.Count; i++)
                {
                    if (!DeviceNames.Any(d => a.bindings[i].effectivePath.Contains(d, StringComparison.CurrentCultureIgnoreCase)))
                        continue;

                    var configItem = Config.Bind($"Bindings", $"{a.bindings[i].action} {i}", $"{a.bindings[i].effectivePath}", new ConfigDescription("", new AcceptableValueList<string>([.. ValidKeyPaths, a.bindings[i].effectivePath])));
                    BindManager binder = new(a, i, configItem);
                }
            }
            else
            {
                if (!DeviceNames.Any(d => a.bindings[0].effectivePath.Contains(d, StringComparison.CurrentCultureIgnoreCase)))
                    return;

                var configItem = Config.Bind($"Bindings", $"{a.bindings[0].action}", $"{a.bindings[0].effectivePath}", new ConfigDescription("", new AcceptableValueList<string>([.. ValidKeyPaths, a.bindings[0].effectivePath])));
                BindManager binder = new(a, 0, configItem);
            }
        });

        ParseAdditiveBinds();
        Config.SettingChanged += OnSettingChanged;
        Config.ConfigReloaded += OnConfigReloaded;
    }

    private void OnConfigReloaded(object sender, EventArgs e)
    {
        Log.LogDebug("Config has been reloaded!");
        BindManager.CheckAll();
    }

    private void OnSettingChanged(object sender, SettingChangedEventArgs settingChangedArg)
    {
        if (settingChangedArg.ChangedSetting == null)
            return;

        if (settingChangedArg.ChangedSetting is ConfigEntry<string> entry)
        {
            if (entry != BindsAdded)
                BindManager.UpdateFromConfig(entry);
            else
                ParseAdditiveBinds();
        }
    }

    private static void ParseAdditiveBinds()
    {
        ManualBind.ClearAll();

        if (string.IsNullOrEmpty(BindsAdded.Value))
            return;

        foreach (var item in BindsAdded.Value.Split(';', System.StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = item.Trim();

            if (string.IsNullOrEmpty(trimmed))
                continue;

            string[] parts = trimmed.Split(':', 2);

            if (parts.Length != 2)
                continue;

            var action = parts[0].Trim();
            var keypath = parts[1].Trim();

            var inputAction = InputSystem.actions.FirstOrDefault(d => d.name == action);

            if (inputAction == null)
                continue;

            ManualBind bind = new(inputAction, keypath); //creates binding and will remove on config item change
        }
    }

    //I originally wrote this for PEAKLib/PEAKTrails but re-using it here
    private static List<string> GenerateValidKeyPaths()
    {
        List<string> result = [];
        List<string> doNotAdd = ["/Keyboard/anyKey"];
        DeviceNames = [];
        Log.LogMessage("Enable debug logging to see ALL valid binding keypaths!");
        foreach (var device in InputSystem.devices)
        {
            if (device == null)
                continue;

            DeviceNames.Add(device.displayName);

            foreach (var control in device.allControls)
            {

                if (doNotAdd.Any(d => d.Equals(control.path, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                string path = control.path.TrimStart('/'); //default bindings usually dont have the starting slash
                Log.LogDebug($"Binding Keypath: {path}");
                result.Add(path);
            }
        }

        return result;
    }
}
