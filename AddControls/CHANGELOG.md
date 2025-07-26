# Changelog

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
