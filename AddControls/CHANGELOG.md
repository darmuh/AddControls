# Changelog

### 0.2.1
- forgot a line to actually make the changes to generative controls, added now :)

### 0.2.0
- Reworked mod to be better applied to all Unity games that utilize the InputAction binding system.
- Updated readme for current version, removed references to PEAK.
- Configuration items with all valid control types are now generated for each input action with a valid device name.
	- This should make the mod work better out of the box as a manual install for any new game.
- You can still manually add controls but default controls can no longer be removed.

### 0.1.1
- Added ParsedInfo class in place of dictionary to allow for binding the same action name to multiple keys
- Consolidated config parsing to ParseConfigItem method
- Updated config parsing method to better handle unexpected formatting or invalid input
- Updated valid values validation to not be case sensitive in AddBind
- Removed some unused usings
- Added check in parsing the config items for duplicate action/key binds
- Added a slightly modified InitBindingsValidation from Permamiss' PR

### 0.1.0
 - Initial release
