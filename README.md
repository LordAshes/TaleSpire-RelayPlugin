# Replay Plugin

This unofficial TaleSpire allows creation of triggers that cause effects to occur. For example,
a level or chain can cause a light to go on or off or a level can open a door. 

Video Demo: https://youtu.be/KOWhUP4i_yw

## Change Log

```
1.2.0: Migrated to Asset Data Plugin to support both Asset Data and Legacy Stat Messaging
1.1.0: Added relay supression to avoid relay reactions on top of relay reactions
1.0.0: Initial release
```

## Install

Use R2ModMan or similar installer to install this plugin.

Set the desired setting using the R2ModMan config for the plugin or keep the default settings.


## Usage

Warning: This plugin does not work with Core TS props, you need to use a custom asset!

1. Select the triggering asset.
2. Use the EAR Animation keys ALT+1 to ALT+7 to choose the desired state. 
3. Relay Plugin will trigger the corresponding effect or effects.
4. Relay Plugin will suspend reaction processing for a time interval to prevent any
   effect changes from triggering reactions (thus causing chain reactions)

The time interval in which relay Plugin ignores reactions if configurable in the R2ModMan
configuration for the Relay Plugin. The value is the number of second in which Relay Plugin
will not process any reactions.

Create a file with the extension SUB in a File Access Plugin valid location. This files defines
the triggers and effects that the realy plugin will implement. The format for the files is as
follows:

```WatchKey:WatchAssetName:State|Delay:SetKey:SetAssetName:State```

or

```WatchKey:WatchAssetName:State|Delay:SetKey:SetAssetName:~MaxStates```

The file contains one trigger condition per line with no blank lines. Each triggering condition
can appear only once in the file but the effect can have more than one effect. To specify more
than one effect (you can have as many as you want), just add more effects starting with the delay
such as:

```WatchKey:WatchAssetName:State|Delay1:SetKey1:SetAssetName1:State1:Delay2:SetKey2:SetAssetName2:State2```

*WatchKey* is the StatMessaging key that is to be watched for the given asset and state.
*WatchAssetName* is the name of the asset who needs to be changed to the specified state in order to trigger
the effects.
*WatchState* is the state that the watched asset need to change to in order to trigger the effects. 
*Delay* is the amount of seconds (can be decimal) that the effect is delayed after the conditions are met.
*SetKey* is the StatMessaging key that is set when the triggering conditions are met.
*SetAssetName* is the name of the asset whose state is set when the triggering conditions are met.
*State* is the value that is set when the triggering conditions are met.

If the *State* starts with a ~ character, it denotes a toggle. The ~ is followed by the maximum number of
states in the toggle. For example, ~2 means toggling between state 1 and 2. Where as ~3 would mean toggling
between state 1, 2 and 3. It should be noted that toggle is 1 indexed (so 1 and 2 and not 0 and 1).

The following example SUB file is included with the plugin:

```
org.lordashes.plugins.extraassetsregistration.Anim:Lever10:1|1.5:org.lordashes.plugins.light:Light01:Torch
org.lordashes.plugins.extraassetsregistration.Anim:Lever10:2|1.5:org.lordashes.plugins.light:Light01:
```

When Lever10 is set to 1, Light01 is set to Torch (turns on a Light using the Light Plugin).
When Lever10 is set to 2, Light01 is set to blank (turns off a Light using the Light Plugin).

### Asset Data vs Legacy Stat Messaging

Relay Plugin has been updated to use Asset Data instead of Stat Messaging. This allows trigger of plugins that
use either Asset Data Plugins or Stat Messaging. The trigger can be either source without needing to tell the
Relay plugin if it is a Asset Data Plugin source or a Legacy Stat Messaging Plugin source. However, the output
needs to indicate if Asset Data Plugin should be used to trigger the output or if the output is a Legacy Stat
Message key. By default, the plugin assumes a Asset Data output. To specify a Legacy Stat Messaging output
preceed the key with an asteriks. For example:

Asset Data Plugin output:
``org.lordashes.plugins.extraassetsregistration.Anim:Lever10:1|1.5:org.lordashes.plugins.light:Light01:Torch``

Legacy Stat Messaging Output:
``org.lordashes.plugins.extraassetsregistration.Anim:Lever10:1|1.5:\*org.lordashes.plugins.light:Light01:Torch``




