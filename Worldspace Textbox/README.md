## Worldspace Textbox ##

###### _Last checked with Unity 5.5.4p3_ ######

This worldspace textbox system allows you to create RPG-style text interactions that exist in world space. It was created for use in _Fred's Happy Factory_, a team-based course project for Virtual Reality at the University of Arkansas at Little Rock in fall 2016, and is based on gamesplusjames' excellent tutorial on creating a textbox ([Part One](https://www.youtube.com/watch?v=ehmBIP5sj0M), [Part Two](https://www.youtube.com/watch?v=7KNQYPcx-uU), [Part Three](https://www.youtube.com/watch?v=vdSxOttY3zg)). However, I tested it on the HTC Vive and quickly found that HMDs have no "screen space", meaning that, for VR, UI elements must appear in world space. I modified the original textbox system to achieve this and to enable multiline dialogue.

I do not recommend using this with non-VR applications, as screenspace textboxes are easier to see in that case.

As with the other Unity tools I've posted here, the `Assets` directory contains all of the assets that will be loaded if you import `WorldspaceTextbox01-Assets.unitypackage` into a Unity project; this includes the sample scene "WorldspaceTextbox01", in `Assets/Scenes/`.

This scene contains three NPCs with slightly different sorts of interactions. One repeats the same dialogue every time you talk to it. Another only says its line once and then does not speak thereafter. The last one utters one line once and then will repeat a second line every time you talk to it again. (There is also a fourth, deactivated NPC for testing scrolling functionality, but I haven't quite gotten that working yet.)

**_Screenshot Placeholder_**

* * *

### How to Build Worldspace Textboxes ###

1. Add your player object with the HTC Vive camera rig as a child. Add the `PlayerController` script and an audio source to the player object. In the `Player Controller` component, you may leave `Input Master` blank; it will autofill if there is an `InputMaster` script attached to an object in the scene. Set `Move Speed` and `Rotate Speed` to your liking. `Can Move` is used internally; leave it checked. `Interact Distance` determines how close you must be, in scene units, to an object in order to interact with it via the interact button (defaulted to the space bar); I've used 3. `Interact Cooldown` is how many seconds must pass between interactions via the interact button; this prevents the player from triggering further interactions before they can lift their finger from the button, and I've found 0.1 to be a good value. Add Vive camera rig's "Camera (head)" object as `Hmd Camera`. Checking `Is Mute` will stop the player from producing any audio during dialogue.

   `Speech Clips` is an array of `SpeechClip` objects. Each of these contains `Clip` specifying an audio clip, `Volume` percentage, and `Probability Weight`. When selecting a clip to play, `PlayerController` randomly chooses one of these speech clips with equal probability and then generates a random value on the range [0, 1]. If this value is less than or equal to `Probability Weight`, the clip is used; if not, `PlayerController` chooses a random clip again.

2. Add the `InputMaster` script as a component to some object in the scene. I usually make a "ScriptMaster" to hold scripts that do not belong to any particular game object. At the very least, ensure that `Using Keyboard Input` is checked. Add the left and right Vive controllers for `Vir Left` and `Vir Right`, respectively.

3. Add the `TextboxManager` script as a component to your script master or to some other object in the scene. You may leave the `Input Master` field blank if you wish; it will autofill. It is also not necessary to fill `Default Textbox Panel`, `Default Textbox`, `Default Animator`, or `Default Audio Source`. `Audio Cutoff Fade Duration` determines over how many seconds audio clips fade if cut off during dialogue; I suggest a value of at least 0.2 to avoid hurting the player's ears with sudden silence. `Text File`, `Current Line`, `End At Line`, `Current Subline`, and `Current Pose` change with different dialogues and may be left blank. Set `Player` to your player object. Checking `Disable Textbox After Audio` will cause textboxes to disappear after an audio clip plays during dialogue; `Textbox Timeout After Audio` is the number of seconds that they will remain. `Textbox Active` is used internally. Checking `Stop Player` will hold the player in place so long as a textbox is open; note that time still passes in the world if the player is stopped. `Textbox Interact Cooldown` is the number of seconds that must pass between presses of the interact button; setting this value too low will result in the player's accidentally filling or disabling textboxes too early. If `Scroll Text` is checked, text will spell out character-by-character in the textbox; if it is not checked, text will display all at once. `Write Speed` is the number of seconds between the output of each character when `Scroll Text` is checked. `Player Script Name` is the name that you use for your player character in dialogue scripts; the default is `PLAYER`.

4. Place NPCs in the style of the NPCCube prefab. Make sure that each NPC has a child Canvas, itself with a child Panel, itself with a child Text, and that these are appropriately sized; the positions of these UI objects is determined dynamically upon interacting with the NPC. Also ensure that the NPC object is tagged as "HasText".

5. Add the `ActivateText` script to each NPC that you want to have a textbox. Set `Player` to your player object. You may leave `Textbox Manager` blank if you wish, as it will autofill. You may also leave `Textbox` blank since it will autofill with the `Text` child you added earlier. Do, however, add the "Panel" and "Canvas" children in the `Textbox Panel` and `Canvas` fields, respectively. You may leave `Animator` and `Audio Source` blank; an animator is not required, but an audio source is, and this will autofill if there is one attached to the same object. `Default Animation Name` will activate the specified animation when the object is created if an animator has been provided.

6. Add texts in the `Texts` array. Uncheck `Is Active` if you do not want the NPC to use this dialogue; this value may be set during gameplay. `Text` is a plaintext file in the format specified below this how-to section; it represents one set of lines for a dialogue. `Current Line` is used internally but also may be set to specify a starting line (lines and sublines are 0-indexed). `End At Line` specifies the last line to use in the dialogue; a value of 0 indicates the end of the text file. Check `One Time Use` if you want the dialogue to be used only once. `Line Separator` and `Subline Separator` define how lines and sublines are delimited in the text file and default to `::` and `,,`, respectively.

7. For each text in the array, set `Poses` if so desired. Each pose will activate when a new line of dialogue appears, including the first one. `Audio Clip` is the audio clip that will play for this pose. `Position Change` will instantaneously change the NPC's position by the indicated values; use this if activating an animation that has a different centerpoint from the previous one. `Angle To Player` is the angle, in degrees, that the NPC will face away from the player; use 0 to have the NPC face the player. `Rotation Speed` determines how fast the NPC rotates to the specified angle. If `Return To Original Facing` is checked, after the line is finished, the NPC will rotate to face the same direction they were facing before the line. Use `Final Pose` to use this pose as the last one for the dialogue, even if there are others after it. Check `Override Default Animation` to override the default animation for the `Activate Text` component using the animator condition specified by `Condition Name`. Checking `Reset Condition After Pose` will cause the animation to reset after the line of dialogue.

8. Start the scene. Walk up to an NPC and press the interact button (defaulted to the space bar on the keyboard and the left trigger on the Vive; controls can be changed in the `InputMaster` script). Continue pressing the interact button to iterate through lines and sublines. Pressing the interact button before a text has finished printing in the textbox will cause the rest of it to fill in immediately. Pressing the cancel button (defaulted to escape on the keyboard and left grip on the Vive) at any time during dialogue will cause the textbox to deactivate.

* * *

### Dialogue Text File Formatting ###

_To be added later. For now, see the included samples in `Assets/Text/`._