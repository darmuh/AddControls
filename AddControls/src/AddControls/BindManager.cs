using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

namespace AddControls;

public class BindManager
{
    internal static List<BindManager> InputActionConfigs = [];
    public InputAction Action;
    public int BindIndex;
    public ConfigEntry<string> Setting;

    public BindManager(InputAction action, int bindingIndex, ConfigEntry<string> config)
    {
        Action = action;
        BindIndex = bindingIndex;
        Setting = config;
        InputActionConfigs.Add(this);
    }

    public static bool TryGetFromConfig(ConfigEntry<string> config, out BindManager result)
    {
        result = InputActionConfigs.FirstOrDefault(item => item.Setting == config);

        return result != null;
    }

    public static bool IsThisExistingConfigItem(InputAction action, string value)
    {
        return InputActionConfigs.Any(x => x.Action == action && x.Setting.Value == value);
    }

    public static void UpdateFromConfig(ConfigEntry<string> entry)
    {
        if (TryGetFromConfig(entry, out BindManager result))
            result.UpdateActionBinding();
    }

    public void UpdateActionBinding()
    {
        Action.ChangeBinding(BindIndex).WithPath(Setting.Value);
    }

    public static void CheckAll()
    {
        foreach (var item in InputActionConfigs)
        {
            if (item.Setting.Value != item.Action.bindings[item.BindIndex].effectivePath)
                item.UpdateActionBinding();
        }
    }
}
