
# ESO SavedVariables Auto Backup [![GitHub license](https://img.shields.io/github/license/FAR747/ESO_SavedVariables_Auto_Backup)](https://github.com/FAR747/ESO_SavedVariables_Auto_Backup/blob/master/LICENSE)
### The program for working with backups SavedVariables of The Elder Scrolls Online.
You can not only create backups manually, but also automatically. For example, when starting a program or exiting the game.
ESVAB is very simple and does not require a lot of system resources.

#### System requirements:

 - Windows 7, 8 and Windows 10 *(Tested only on Win 10)*
 - .NET Framework 4.7.2+

####How to install:

 1. Download the archive from the Releases and unpack it to a convenient place. (I highly **recommend** just putting it in the folder `/Documents /Elder Scrolls Online/`)
 2. Run `ESO SavedVariables Auto backup.exe`
 3. In the window that opens, *click next*
 4. Select the Elder Scrolls Online directory in the Documents directory. The program itself should find this directory and show the ESO folders *(live, pts and etc.)* *Click Next*
 5. Here you need to select the location where the backups will be saved. I advise you to leave it as it is.
Below, select the ESO folders from which the settings will be backed up. (By default, all folders are selected, but note that you cannot uncheck the `live` folder) *Click Next*
6. All setup is complete. Now *click Finish*

####How to set up automatic backup:

 1. Click on `File` at the top and select` Settings`
 2. In the window that opens, you will find the automatic backup settings:
  `Automatic backup at program start` - If enabled, the program will create backups each time it starts. Works well with the parameter `Run the program at Windows startup`
  `Automatic backup when closing ESO` - If enabled, the program will create backups some time after the game is closed.
