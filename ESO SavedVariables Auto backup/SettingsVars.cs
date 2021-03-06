﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IniParser;
using IniParser.Model;

namespace ESO_SavedVariables_Auto_backup
{
	class SettingsVars
	{
		public static string DefFolderName = "ESO SV Auto Backup";
		public static string ESODir = "none";
		public static string Backupdir = "none";
		public static List<SVProfile> Profiles = new List<SVProfile>();
		public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public static string configpath = appdata + "/ESVAB.cfg";
		static int Sav_Version = -1;

		#region settings
		public static bool autodeletebackups = false;
		public static int maxdaybackup = 30;
		public static bool ESORunning_MessageDisable = false;

		public static bool autobackup_startup = false;
		public static bool autobackup_exitESO = false;
		#endregion settings

		public static void firstsettings_finish()
		{
			if (!Directory.Exists(Backupdir))
			{
				Directory.CreateDirectory(Backupdir);
			}
			IniData gConfig = new IniData();
			gConfig.Sections.AddSection("config");
			gConfig["config"].AddKey("ESODir", ESODir);
			gConfig["config"].AddKey("BackupDir", Backupdir);
			gConfig["config"].AddKey("ESORunning_MessageDisable", "false");
			gConfig["config"].AddKey("Version", MainWindow.VERSION_CODE.ToString());
			

			gConfig["config"].AddKey("autobackup_startup", "false");
			gConfig["config"].AddKey("autobackup_exitESO", "false");

			FileIniDataParser parser = new FileIniDataParser();
			parser.WriteFile(appdata + "/ESVAB.cfg", gConfig);
		}
		public static void LoadConfig()
		{
			if (checkExistConf())
			{
				bool savecfg = false;
				FileIniDataParser parser = new FileIniDataParser();
				IniData gConfig = parser.ReadFile(configpath);
				ESODir = gConfig["config"]["ESODir"];
				Backupdir = gConfig["config"]["BackupDir"];
				Sav_Version = Convert.ToInt32(gConfig["config"]["Version"]);

				if (gConfig["config"]["autobackup_startup"] != null)
				{
					autobackup_startup = bool.Parse(gConfig["config"]["autobackup_startup"]);
				}
				else
				{
					savecfg = true;
				}
				if (gConfig["config"]["autobackup_exitESO"] != null)
				{
					autobackup_exitESO = bool.Parse(gConfig["config"]["autobackup_exitESO"]);
				}
				else
				{
					savecfg = true;
				}
				if (gConfig["config"]["autodeletebackups"] != null)
				{
					autodeletebackups = bool.Parse(gConfig["config"]["autodeletebackups"]);
				}
				else
				{
					savecfg = true;
				}
				if (gConfig["config"]["autodeletebackups"] != null)
				{
					maxdaybackup = Convert.ToInt32(gConfig["config"]["maxdaybackup"]);
				}
				else
				{
					savecfg = true;
				}
				if (gConfig["config"]["ESORunning_MessageDisable"] != null)
				{
					ESORunning_MessageDisable = bool.Parse(gConfig["config"]["ESORunning_MessageDisable"]);
				}
				else
				{
					savecfg = true;
				}
				if (savecfg)
				{
					SaveConfig();
				}
			}
		}
		public static void SaveConfig()
		{
			IniData gConfig = new IniData();
			gConfig.Sections.AddSection("config");
			gConfig["config"].AddKey("ESODir", ESODir);
			gConfig["config"].AddKey("BackupDir", Backupdir);
			gConfig["config"].AddKey("Version", MainWindow.VERSION_CODE.ToString());

			gConfig["config"].AddKey("autobackup_startup", autobackup_startup.ToString());
			gConfig["config"].AddKey("autobackup_exitESO", autobackup_exitESO.ToString());
			gConfig["config"].AddKey("autodeletebackups", autodeletebackups.ToString());
			gConfig["config"].AddKey("maxdaybackup", maxdaybackup.ToString());
			gConfig["config"].AddKey("ESORunning_MessageDisable", ESORunning_MessageDisable.ToString());
			FileIniDataParser parser = new FileIniDataParser();
			parser.WriteFile(configpath, gConfig);
		}
		public static void LoadProfiles()
		{
			if (!Directory.Exists(Backupdir))
			{
				System.Windows.MessageBox.Show(String.Format("A fatal error has occurred. Backup directory ({0}) not found!\n\nThe program settings will be reset. You will need to re-configure the program.", Backupdir), "Critical Error",System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
				File.Delete(configpath);
				MainWindow.closeprogramm();
				return;
			}
			string[] dirs = Directory.GetDirectories(Backupdir);
			Profiles.Clear();
			foreach (string dir in dirs)
			{
				string manifestFile = dir + "/backupdirectory.cfg";
				if (File.Exists(manifestFile))
				{
					FileIniDataParser parser = new FileIniDataParser();
					IniData Config = parser.ReadFile(manifestFile);
					SVProfile SVP = new SVProfile();
					SVP.Name = Config["config"]["Name"];
					SVP.Path = Config["config"]["Path"];
					Profiles.Add(SVP);
					AutoBackups.checkoldbackups(SVP); //CHECK OLD BACKUPS
				}
			}
		}
		public static bool checkExistConf()
		{
			if (File.Exists(configpath))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public static void addprofile(SVProfile SVP)
		{
			bool err = false;
			foreach (SVProfile cSVP in Profiles)
			{
				if (cSVP.Path == SVP.Path)
				{
					err = true;
				}
			}
			if (!err)
			{
				string Backdirectory = Backupdir + "\\" + SVP.Name;
				if (!Directory.Exists(Backdirectory))
				{
					Directory.CreateDirectory(Backdirectory);
				}
				IniData bConfig = new IniData();
				bConfig.Sections.AddSection("config");
				bConfig["config"].AddKey("ESODir", ESODir);
				bConfig["config"].AddKey("LastBackup", "0");
				bConfig["config"].AddKey("Name", SVP.Name);
				bConfig["config"].AddKey("Path", SVP.Path);
				FileIniDataParser parser = new FileIniDataParser();
				parser.WriteFile(Backdirectory + "/backupdirectory.cfg", bConfig);
				Profiles.Add(SVP);
			}
		}
	}
}
