ASA Backup Manager is a tool to automaticaly backup ARK: Survival Ascended world files.

You can add your world files via the 'right-click' context menu then browse to find your .ARK file
![image](https://github.com/user-attachments/assets/d15fe669-df0f-4665-84cd-e755224939a3)


Once added you can specify the number of backup files you wish to keep and as backups occur the oldest
will be deleted as needed. The backup files are saved in the same folder as the .ARK file by default and are 
embedded in a .ZIP file along with any .ARKProfile and .ARKTribe files in the same folder. Version 1.1
allows you to change the backup path from the default location. 

<i>NOTE: If you are backing up multiple instances of the same world (eg. TheIsland.ark) you should not have them
all backup to the same path since the backup manager won't know the difference between them and WILL incorrectly
remove backups of the oldest one. If your world files have unique names (eg. TheIsland1.ark, TheIsland2.ark, etc)
then this will not be a concern.</i>

There is no need to configure the frequency of the backup, a fresh backup will occur automatically
every time the world is saved (default of 15 minutes). 

While ASABM was intended for servers it SHOULD work to back up your single player worlds but has only
been lighlty tested using the Steam version of ASA. It MIGHT also work for ASE games but has not
been validated. 

ASA Backup Manager requires Microsoft .Net 4.7.2 or higher to run and has been tested on both
Windows 10 and Windows 11.


About Me:
I have been a hobby devloper for over 20 years mostly in C++ and C#. This would mark the first 
tool I have devloped and released to the public but have countless others for peronal use
and/or colleagues at work.
