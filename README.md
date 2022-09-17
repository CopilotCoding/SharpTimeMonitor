# TimeMonitorSharp
This program records the amount of time your PC spends in a wake state, how long it spends in a sleep state, the date of when it woke or slept, and a current session timer.
It also records the time since the computer was last restarted.
Useful for people who have no idea how much time they spend on their computer, or away from it.

The delay is set to every 10 seconds for it to write to the file so as to minimize resource usage.

# Requirements before building
In the documentation there was a mention of the function used in this program not working unless the console program is a background program, which means you must change the project properties from console to windows application.
