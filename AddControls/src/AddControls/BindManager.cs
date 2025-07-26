using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using static AddControls.Misc;
using static AddControls.Plugin;
using static UnityEngine.Rendering.DebugUI;

namespace AddControls
{
    public class ParsedInfo(string actionName, string keyPath)
    {
        public string ActionName { get; set; } = actionName;
        public string KeyPath { get; set; } = keyPath;
    }

    public class BindManager
    {
        internal static List<ParsedInfo> customBindings = [];
        internal static List<ParsedInfo> removeList = [];

        public static void Start()
        {
            Log.LogMessage("BindManager Start initialized!");
            CustomBindings();
            RemoveBindings();
            Log.LogMessage("BindManager Start complete!");
        }

        public static void CustomBindings()
        {
            InitCustomBindings();
            if (customBindings.Count > 0)
            {
                customBindings.Do(c => AddBind(c.ActionName, c.KeyPath));
            }
        }

        public static void RemoveBindings()
        {
            InitBindsToRemove();
            if (removeList.Count > 0)
            {
                removeList.Do(r => RemoveBind(r.ActionName, r.KeyPath));
            }
        }

        private static bool ParseConfigItem(ref List<ParsedInfo> customList, string rawValue)
        {
            customList = [];

            if (string.IsNullOrEmpty(rawValue))
                return false;
            else
            {
                if (string.IsNullOrEmpty(rawValue))
                    return false;

                foreach(var item in rawValue.Split(';', System.StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimmed = item.Trim();

                    if (string.IsNullOrEmpty(trimmed))
                        continue;

                    string[] parts = trimmed.Split(':', 2);
                    
                    if (parts.Length != 2)
                        continue;

                    var action = parts[0].Trim();
                    var keypath = parts[1].Trim();

                    if (customList.Any(bind => bind.ActionName == action && bind.KeyPath == keypath))
                        continue;

                    ParsedInfo parsedInfo = new(action, keypath);
                    customList.Add(parsedInfo);
                }
            }

            return customList.Count > 0;
        }

        private static void InitCustomBindings()
        {
            if(!ParseConfigItem(ref customBindings, bindsAdded.Value))
                Log.LogDebug("No custom binds to add!");
            else
                Log.LogDebug("Custom Bindings list created!");
        }

        private static void InitBindsToRemove()
        {
            removeList = [];

            if (!ParseConfigItem(ref removeList, bindsRemoved.Value))
                Log.LogDebug("No binds to remove!");
            else
                Log.LogDebug("removeList created!");
        }

        //slightly modified from PR https://github.com/Permamiss/AddControls/tree/master
        public static void InitBindingsValidation()
        {
            if (customBindings.Count == 0 || removeList.Count == 0)
                Log.LogDebug("No custom binds or removed binds to compare against!");
            else
                removeList.DoIf(bind => customBindings.Any(v => v.ActionName == bind.ActionName && v.KeyPath == bind.KeyPath), bind => customBindings.Remove(bind));

            Log.LogDebug("Custom Bindings list & removeList validated!");
        }

        //method used to actually update input action after validations
        private static void AddActionBind(InputAction inputAction, string Name, string Value)
        {
            if (inputAction == null)
            {
                Log.LogError($"Unable to find inputAction for {Name} @ AddAction!");
                return;
            }

            inputAction.AddBinding(Value);
            Log.LogMessage($"Added binding for {Name} with {Value}");
        }

        //method used to actually clear an existing binding
        private static void RemoveActionBind(InputAction inputAction, string Name, string Value)
        {
            if (inputAction == null)
            {
                Log.LogError($"Unable to find inputAction for {Name} @ RemoveAction!");
                return;
            }

            inputAction.ChangeBindingWithPath(Value).Erase();
            Log.LogMessage($"Removed binding for {Name} with {Value}");
        }

        //bind removal entry point
        public static void RemoveBind(string actionName, string value)
        {
            InputAction inputAction = InputSystem.actions.FindAction(actionName);

            if (inputAction == null)
            {
                Log.LogWarning($"Unable to remove bind {actionName} to {value} (INVALID ACTION)");
                return;
            }

            if (!inputAction.bindings.Any( b => b.effectivePath.Contains(value, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                Log.LogWarning($"Unable to remove bind {actionName} from {value}, this is not already bound!");
                return;
            }

            RemoveActionBind(inputAction, actionName, value);
        }

        //bind add entry point
        public static void AddBind(string actionName, string value)
        {
            InputAction inputAction = InputSystem.actions.FindAction(actionName);

            if(inputAction == null)
            {
                Log.LogWarning($"Unable to bind {actionName} to {value} (INVALID ACTION)");
                return;
            }

            if (!ValidValues.Any(v => v.Equals(value, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                Log.LogWarning($"Unable to bind {actionName} to {value} (INVALID VALUE)");
                return;
            }

            AddActionBind(inputAction, actionName, value);
        }
    }
}
