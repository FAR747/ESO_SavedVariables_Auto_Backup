using System;
using System.Collections.Generic;
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
				Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				string name = String.Format("AutoBackup_{0}", unixTimestamp);
				//Backups.Create(MainWindow.LoadedProfile, name, true);
				foreach (SVProfile SVP in SettingsVars.Profiles)
				{
					Backups.Create(SVP, name, true);
				}
			}
			if (SettingsVars.autobackup_exitESO)
			{
				ESORunned = CheckESO();
				System.Diagnostics.Debug.WriteLine("ESO Status: " + ESORunned);
			}
			TimerCallback tm = new TimerCallback(Timer_tick);
			timer = new Timer(tm, 0, 0, 5000); //2mins 120000
		}

		static void Timer_tick(object obj)
		{
			if (SettingsVars.autobackup_exitESO)
			{
				bool currentESORunned = CheckESO();
				if (ESORunned && !currentESORunned)
				{
					ESORunned = currentESORunned;
					System.Diagnostics.Debug.WriteLine("ESO Status: " + "EXIT");
					Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
					string name = String.Format("AutoBackup_{0}", unixTimestamp);
					//Backups.Create(MainWindow.LoadedProfile, name, true);
					foreach (SVProfile SVP in SettingsVars.Profiles)
					{
						Backups.Create(SVP, name, true);
					}
				}
				else if (currentESORunned && !ESORunned)
				{
					ESORunned = currentESORunned;
					System.Diagnostics.Debug.WriteLine("ESO Status: " + "START");
				}
			}
		}
		static bool CheckESO()
		{
			return System.Diagnostics.Process.GetProcessesByName("eso64").Any();
		}
	}
}
