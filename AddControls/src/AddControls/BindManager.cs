using HarmonyLib;
using pworld.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static AddControls.Misc;
using static AddControls.Plugin;

namespace AddControls
{
	public class BindManager
	{
		internal static Dictionary<string, string> customBindings = [];
		internal static Dictionary<string, string> prevCustomBindings = [];
		internal static Dictionary<string, string> removeList = [];
		internal static Dictionary<string, string> prevRemoveList = [];

		public static void Start()
		{
			Log.LogMessage("BindManager Start initialized!");
			RemoveBindings();
			CustomBindings();
			Log.LogMessage("BindManager Start complete!");
		}

		public static void CustomBindings()
		{
			InitPrevCustomBindings();
			InitCustomBindings();
			
			if (prevCustomBindings.Count > 0)
				prevCustomBindings.Do(bind => RemoveBind(bind.Key, bind.Value));
			if (customBindings.Count > 0)
				customBindings.Do(bind => AddBind(bind.Key, bind.Value));
		}

		public static void RemoveBindings()
		{
			InitPrevBindsToRemove();
			InitBindsToRemove();
			
			if (prevRemoveList.Count > 0)
			{
				prevRemoveList.Do(bind => AddBind(bind.Key, bind.Value));
			}
			if (removeList.Count > 0)
			{
				removeList.Do(bind => RemoveBind(bind.Key, bind.Value));
			}
		}

		private static void InitCustomBindings()
		{
			customBindings = [];

			if (string.IsNullOrEmpty(bindsAdded.Value))
				Log.LogDebug("No custom binds to add!");
			else
			{
				if (bindsAdded.Value.EndsWith(';'))
					bindsAdded.Value = bindsAdded.Value.TrimEnd(';');

				customBindings = bindsAdded.Value
					.Split(';')
					.Select(item => item.Trim())
					.Select(item => item.Split(':'))
					.ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim());
			}

			InitBindingsValidation();

			Log.LogDebug("Custom Bindings list created!");
		}

		private static void InitPrevCustomBindings()
		{
			prevCustomBindings = [];
			prevCustomBindings = customBindings.ToDictionary(bind => bind.Key, bind => bind.Value);

			Log.LogDebug("Previous Custom Bindings list created!");
		}

		private static void InitBindsToRemove()
		{
			removeList = [];

			if (string.IsNullOrEmpty(bindsRemoved.Value))
				Log.LogDebug("No custom binds to add!");
			else
			{
				if (bindsRemoved.Value.EndsWith(';'))
					bindsRemoved.Value = bindsRemoved.Value.TrimEnd(';');

				removeList = bindsRemoved.Value
					.Split(';')
					.Select(item => item.Trim())
					.Select(item => item.Split(':'))
					.ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim());
			}

			InitBindingsValidation();

			Log.LogDebug("removeList created!");
		}

		private static void InitPrevBindsToRemove()
		{
			prevRemoveList = [];
			prevRemoveList = removeList.ToDictionary(bind => bind.Key, bind => bind.Value);

			Log.LogDebug("prevRemoveList created!");
		}

		public static void InitBindingsValidation()
		{
			if (customBindings.Count == 0 || removeList.Count == 0)
				Log.LogDebug("No custom binds or removed binds to compare against!");
			else
				removeList.DoIf(bind => customBindings.Contains(bind), bind => customBindings.Remove(bind.Key));

			Log.LogDebug("Custom Bindings list & removeList validated!");
		}

		//method used to actually update inputaction after validations
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

			//if (!InputSystem.actions.Any(a => a.name == actionName))
			if (inputAction == null)
			{
				Log.LogWarning($"Unable to remove bind {actionName} to {value} (INVALID ACTION)");
				return;
			}

			if (!inputAction.bindings.Any(b => b.effectivePath.Contains(value, System.StringComparison.InvariantCultureIgnoreCase)))
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

			//if (!InputSystem.actions.Any(a => a.name == actionName))
			if(inputAction == null)
			{
				Log.LogWarning($"Unable to bind {actionName} to {value} (INVALID ACTION)");
				return;
			}

			if (!ValidValues.Contains(value))
			{
				Log.LogWarning($"Unable to bind {actionName} to {value} (INVALID VALUE)");
				return;
			}

			//InputAction inputAction = InputSystem.actions.FindAction(actionName);
			AddActionBind(inputAction, actionName, value);
		}
	}
}
