# AddControls

Manage existing controls via generative configuration items. Manually add new bindings for any existing control.   

## Configuration

### Add Custom Bindings
- To add custom bindings, you'll want to modify the ``Binds Added`` configuration item in the ``Manual Binds`` section of the config.
- This configuration item is a single text string that will be parsed for your desired input action and keybinding.
- The format is as follows: ``actionName:keybindingPath;actionName2:keybindingPath2``
- For example: if you wanted to bind the action, ``Crouch``, to the key, ``C``, and the action, ``Jump``, to the mouse ``Scroll Wheel`` you would do ``Crouch:<Keyboard>/c;Jump:<Mouse>/scroll``
- If this config item is changed mid-game, any previous added keybinds will be cleared.

### Action Names and Keybind Paths
- For a list of acceptable actions and keybind paths, please enable debug logging and check your LogOutput.Log after launching once.
- NOTE: There is no limit to how many binds you can add. Just continue to follow the correct formatting for each new action/keybind.
