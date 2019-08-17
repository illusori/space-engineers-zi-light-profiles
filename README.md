# space-engineers-zi-light-profiles
Space Engineers - Zephyr Industries Light Profiles

## Living in the Future(tm)

*Ever wanted to change your lights to a new colour scheme, but don't want to lose all your old settings?*

Zephyr Industries has what you need, I'm here to tell you about their great new product: _Zephyr Industries Light Profiles_.

Save your existing light configurations with a press of a button! Configure them to something new! Switch them back and forth! Life has never been so good, that's what Living in the Future(tm) means!

## Instructions:
* Place on a Programmable Block.
* Run the script with the name of a profile as the argument to switch to that profile.
* When you switch profile, the current light settings are saved to the old profile for that light. (In the case of messing around with subgrids and docking and other stuff, this should "do the right thing".)
* If the light is new to the script, it will save under the "original" profile.
* Scans for new lights every 300 game updates. ~30-60s.
* Saves and restores radius, intensity, falloff, blink interval, blink length, blink offset, light offset, colour and on/off state.

## Notes:
* Messes with CustomData on the light, other scripts that mess with this may clash.
* Try the "Capac" profile for some fun times and the inspiration to finally get around to writing this script.
* Set your light profiles to different intensities and put them on a button bar: you now have a dimmer switch.
* Use sensors to change to an "alert" profile when intruders are detected, and reset again afterwards.
