using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESO_SavedVariables_Auto_backup
{
	class AutoBackups
	{
		static Timer timer;
		public static bool ESORunned = false;
		public static void init()
		{
			if (SettingsVars.autobackup_startup)
			{
				Int64 unixTimestamp = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				string name = String.Format("AutoBackup_{0}", unixTimestamp);
				//Backups.Create(MainWindow.LoadedProfile, name, true);
				foreach (SVProfile SVP in SettingsVars.Profiles)
				{
					Backups.Create(SVP, name, true);
				}
			}
			ESORunned = CheckESO();
			System.Diagnostics.Debug.WriteLine("ESO Status: " + ESORunned);
			TimerCallback tm = new TimerCallback(Timer_tick);
			timer = new Timer(tm, 0, 0, 10000); //2mins 120000
		}

		static void Timer_tick(object obj)
		{
			
				bool currentESORunned = CheckESO();
				if (ESORunned && !currentESORunned)
				{
					ESORunned = currentESORunned;
					System.Diagnostics.Debug.WriteLine("ESO Status: " + "EXIT");
					if (SettingsVars.autobackup_exitESO)
					{
						Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
						string name = String.Format("AutoBackup_{0}", unixTimestamp);
						//Backups.Create(MainWindow.LoadedProfile, name, true);
						foreach (SVProfile SVP in SettingsVars.Profiles)
						{
							Backups.Create(SVP, name, true);
						}
					}
				}
				else if (currentESORunned && !ESORunned)
				{
					ESORunned = currentESORunned;
					System.Diagnostics.Debug.WriteLine("ESO Status: " + "START");
				}
		}
		static bool CheckESO()
		{
			bool check = System.Diagnostics.Process.GetProcessesByName("eso64").Any();

			if (check)
			{
				MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gMini_Message.Content = "ESO Running"));
				MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gMini_Message.Visibility = System.Windows.Visibility.Visible));
			}
			else
			{
				MainWindow.gCreateback_Button.Dispatcher.BeginInvoke((Action)(() => MainWindow.gMini_Message.Visibility = System.Windows.Visibility.Hidden));
			}

			return check;
		}

		public static void checkoldbackups(SVProfile Profile)
		{
			if (SettingsVars.autodeletebackups)
			{
				string path = SettingsVars.Backupdir + "\\" + Profile.Name;
				int maxdays = SettingsVars.maxdaybackup * 86400; // 86400 - 1 day
				FileIniDataParser parser = new FileIniDataParser();
				DirectoryInfo files = new DirectoryInfo(path);
				FileInfo[] dFiles = files.GetFiles("*.ESVAB.backup").OrderByDescending(p => p.CreationTime).ToArray();
				foreach (FileInfo file in dFiles)
				{
					string zipname = file.FullName.Replace(".backup", ".zip");
					IniData config = parser.ReadFile(file.FullName);
					Int64 creationdate = Convert.ToInt64(config["config"]["date"]);
					Int64 unixTimestamp = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
					Int64 filelivetime = unixTimestamp - creationdate;
					if (filelivetime > maxdays)
					{
						File.Delete(file.FullName);
						File.Delete(zipname);
					}
				}
			}
		}
	}
}
