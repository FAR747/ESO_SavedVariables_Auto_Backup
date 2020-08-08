using System;
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
			gConfig["config"].AddKey("Version", MainWindow.VERSION_CODE.ToString());
			FileIniDataParser parser = new FileIniDataParser();
			parser.WriteFile(appdata + "/ESVAB.cfg", gConfig);
		}
		public static void LoadConfig()
		{
			if (checkExistConf())
			{
				FileIniDataParser parser = new FileIniDataParser();
				IniData gConfig = parser.ReadFile(configpath);
				ESODir = gConfig["config"]["ESODir"];
				Backupdir = gConfig["config"]["BackupDir"];
				Sav_Version = Convert.ToInt32(gConfig["config"]["Version"]);
			}
		}
		public static void LoadProfiles()
		{
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
