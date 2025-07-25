# AddControls

Add and remove bindings of your own! Allows for binding any control to multiple keys.

## Configuration

### Add Custom Bindings
- To add custom bindings, you'll want to modify the ``Binds Added`` configuration item.
- This configuration item is a single text string that will be parsed for your desired input action and keybinding.
- The format is as follows: ``actionName:keybindingPath;actionName2:keybindingPath2``
- For example: if you wanted to bind the action, ``Crouch``, to the key, ``C``, and the action, ``Jump``, to the mouse ``Scroll Wheel`` you would do ``Crouch:<Keyboard>/c;Jump:<Mouse>/scroll``

### Remove Existing Bindings
- To remove bindings, you'll want to modify the ``Binds Removed`` configuration item.
- This configuration item is a single text string that will be parsed for your desired input action and keybinding.
- The format is as follows: ``actionName:keybindingPath;actionName2:keybindingPath2``
- For example, if you wanted to unbind the action, ``Crouch``, from the key, ``CTRL``, you would do ``Crouch:<Keyboard>/ctrl``

### Action Names and Keybind Paths
- For a list of acceptable actions please see ``Possible Action Names``.
- For a list of acceptable keybinding paths see [KeyBindValues.txt from Peak-Unbound](https://github.com/glarmer/PEAK-Unbound/blob/main/KeyBindValues.txt)
- NOTE: There is no limit to how many binds you can add or remove. Just continue to follow the correct formatting for each new action/keybind.

### Making changes in-game
- If you would like to reload your config items to add/remove bindings and have [ModConfig](https://thunderstore.io/c/peak/p/PEAKModding/ModConfig/) installed, simply change the configuration item labeled ``Reload Settings`` to a different value. 