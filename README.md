at9MarketPauser
===============

A simple application that pauses Atrain 9 when the global stock market values reach a specific threshold (read/write mem and global key hook)


Use:

Open ATrain 9
Load your game file or start a new game
Alt-tab and run marketPauser as an Administrator
Alt tab back to your game

Press Ctrl-PageUp to switch between buy and sell mode
Press Ctrl-PageDown to start or stop the Market Pauser

Troubleshooting:
Global Hotkeys are not working -> Restart the Market Pauser. I have seen this happen in Win8.1 several times (I suspect the global hotkey is not being deregistered)

Hotkeys are working but Market Value is not updating.
Solution 1) Run as administrator
Solution 2) Open Cheat Engine and verify 0x7AAFE0 is still showing the correct number
If it is not, locate the memory location and rebuild with the new offset
Solution 3) Same as solution 2. This was not written for steam versions, nor will it find the offset based on signature. Use Cheat engine to find the proper offsets

Current offsets needed
0x7AAFE0 -> Market Value

(to find market value just keep pausing the game with a different value and search on only positive ints) There will be two statics remaining and either will work.

0x7AA850 -> Speed Multiplier
0x3411864 -> Next Speed
0x3422860 -> Current Speed

(to find the speeds there should be 3 static offsets remaining when you alter speed forward and back checking for increased/decreased value each time... really easy)
