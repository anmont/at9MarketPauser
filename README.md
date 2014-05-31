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
                                                                                                                                                                                                                                                                                                                                                                                                C3 - this is the offset      
00 58 40 FB A2 00 00 00 00 00 00 00 00 00 00 00 A0 86 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF 26 00 00 00 31 00 00 00 A1 00 00 00 C4 02 00 00 D0 05 00 00 C3 FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00

0x7AA850 -> Speed Multiplier
                                                                                                                                                                                                00 - this is the offset                    
7C F9 A5 42 00 00 80 43 00 00 80 43 00 00 80 40 00 00 80 43 00 00 80 43 00 00 80 40 E9 07 00 00 02 00 00 00 0E 00 00 00 0A 00 00 00 2E 00 00 00 25 00 00 00 F4 01 00 00 00 00 00 00 00 00 8C 42 00 00 00 00 D5 02 00 00 A3 04 00 00 7C 92 00 00 00 00 00 00 2E 00 00 00 0A 00 00 00 1C F9 0F 00 00 00 00 00 F4 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 01 00 00 00 03 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00

0x3411864 -> Next Speed
0x3422860 -> Current Speed

                                                                                                00- offset 1 followed by offset 2                                                                    
2E 00 00 00 25 00 00 00 F4 01 00 00 05 00 00 00 0D EB 3B 00 7D 65 71 1C F4 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 EB 43 A3 43 0A 00 00 00 1E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 98 3A 00 00 00 00 00 00 00 00 00 00 78 48 57 2E D0 4D 57 2E D0 4D 57 2E 00 00 00 00 B0 6F 51 2E 88 BD 52 2E 88 BD 52 2E
