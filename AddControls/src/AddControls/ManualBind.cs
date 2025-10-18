using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace AddControls;

public class ManualBind
{
    internal static List<ManualBind> ManualBindings = [];
    public InputAction Action;
    public int AddedBindIndex;
    public string Value;

    public ManualBind(InputAction action, string value)
    {
        Action = action;
        Value = value;
        ManualBindings.Add(this);
        AddBinding();
    }

    public void AddBinding()
    {
        if (BindManager.IsThisExistingConfigItem(Action, Value))
            return;

        Action.AddBinding(Value);
        AddedBindIndex = Action.bindings.IndexOf(b => b.effectivePath.Equals(Value, System.StringComparison.InvariantCultureIgnoreCase));
    }

    public void RemoveBinding()
    {
        Action.ChangeBinding(AddedBindIndex).Erase();
    }

    public static void ClearAll()
    {
        for (int i = ManualBindings.Count - 1; i >= 0; i--)
        {
            ManualBindings[i].RemoveBinding();
        }

        ManualBindings.Clear();
    }
}
