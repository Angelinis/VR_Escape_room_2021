# VR_Escape_room_2021

# Technical Changes

The `Audio_Manager` object possess a script to control all audio objects volume. It also saves some clips from the game as Sfx clips and Description Clips.

The `Ray Interactor` object in the right controller has been changed to have a `Max Raycast` of 0.1. This is inside of XR Ray Interactor Component.

The `Game_Manager` object possess a script that inspects if all elements were picked up. Also, it has a `Play On Spawn (Script)` that checks for button presses to play some tutorial or Description Clips. Finally, it has a `Pause Menu Manager` that is in charge to setactive the `Menu_Canvas` and pause the game.

There is a `Collider_Object_VR_Body` that plays a collision sound when it collides with any other object.
It also has a script to Follow the Camera. It has been changed, so it can follow the camera in all three axis.

The pause `Menu_Canvas` is inside the `Main Camera`, and it is activated through a Menu Button that was set in the `XRI Default Input Actions`. Also, the `Menu_Canvas` has a `Pause Menu Controller` that connects the sliders with the `Audio Manager` instances to reduce volume.

The walking sounds come from the `Dynamic Move Provider (Script)` and the `Edited Action Based Snap Turn Provider (Script)` inside the `XR Origin`. Besides, the last script is based on an `Edited Snap Turn Provider Base (Script)` which was changed and adapted..

Tip: The building plane has a proportion of 6 meters in Real, and 10 in the plane 100%. Which provides a conversion rate of 0.60.
